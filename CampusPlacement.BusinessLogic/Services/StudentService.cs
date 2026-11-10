using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.Students;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.DataAccess.Context;
using CampusPlacement.Domain.Constants;
using CampusPlacement.Domain.Entities;
using CampusPlacement.Domain.Enums;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacement.BusinessLogic.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public StudentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
    {
        _context = context;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<StudentResponseDto> EnrollAsync(EnrollStudentDto dto)
    {
        var collegeId = _currentUserService.CollegeId!.Value;
        var officerId = _currentUserService.ProfileId!.Value;

        if (await _context.Students.AnyAsync(s => s.CollegeId == collegeId && s.RegisterNumber == dto.RegisterNumber))
            throw new ConflictException("A student with this register number already exists in your college.");

        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            throw new ConflictException("An account with this email already exists.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                AccountStatus = AccountStatus.Pending
            };

            // Generating a default temporary password for enrolled students
            var createResult = await _userManager.CreateAsync(user, "Student@123!");
            if (!createResult.Succeeded)
                throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, RoleConstants.Student);

            var student = new Student
            {
                UserId = user.Id,
                CollegeId = collegeId,
                PlacementOfficerId = officerId,
                RegisterNumber = dto.RegisterNumber,
                Department = dto.Department,
                Degree = dto.Degree,
                GraduationYear = dto.GraduationYear,
                CGPA = dto.CGPA,
                ActiveBacklogs = dto.ActiveBacklogs,
                Status = AccountStatus.Pending
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var collegeName = await _context.Colleges.Where(c => c.CollegeId == collegeId).Select(c => c.CollegeName).FirstAsync();
            return MapToDto(student, user, collegeName);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<BulkEnrollmentResultDto> BulkEnrollAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null.");

        var collegeId = _currentUserService.CollegeId!.Value;
        var officerId = _currentUserService.ProfileId!.Value;
        var result = new BulkEnrollmentResultDto();

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

        foreach (var row in rows)
        {
            try
            {
                var dto = new EnrollStudentDto
                {
                    FullName = row.Cell(1).GetValue<string>(),
                    RegisterNumber = row.Cell(2).GetValue<string>(),
                    Email = row.Cell(3).GetValue<string>(),
                    PhoneNumber = row.Cell(4).GetValue<string>(),
                    Department = row.Cell(5).GetValue<string>(),
                    Degree = row.Cell(6).GetValue<string>(),
                    GraduationYear = row.Cell(7).GetValue<int>(),
                    CGPA = row.Cell(8).GetValue<decimal>(),
                    ActiveBacklogs = row.Cell(9).GetValue<int>()
                };

                await EnrollAsync(dto);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add($"Row {row.RowNumber()}: {ex.Message}");
            }
        }

        return result;
    }

    public async Task<IEnumerable<StudentResponseDto>> GetAllAsync()
    {
        var query = _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .AsNoTracking();

        // Security Isolation: Placement Officers only see their own students
        if (_currentUserService.Role == RoleConstants.PlacementOfficer)
        {
            query = query.Where(s => s.CollegeId == _currentUserService.CollegeId);
        }

        var students = await query.ToListAsync();
        return students.Select(s => MapToDto(s, s.User, s.College.CollegeName));
    }

    public async Task<IEnumerable<StudentResponseDto>> GetPendingAsync()
    {
        var students = await _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .Where(s => s.CollegeId == _currentUserService.CollegeId && s.Status == AccountStatus.Pending)
            .AsNoTracking()
            .ToListAsync();

        return students.Select(s => MapToDto(s, s.User, s.College.CollegeName));
    }

    public async Task<StudentResponseDto> GetByIdAsync(int id)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
            throw new NotFoundException($"Student with ID {id} not found.");

        if (_currentUserService.Role == RoleConstants.PlacementOfficer && student.CollegeId != _currentUserService.CollegeId)
            throw new UnauthorizedAccessException("You are not authorized to view this student.");

        return MapToDto(student, student.User, student.College.CollegeName);
    }

    public async Task<StudentResponseDto> GetByUserIdAsync(string userId)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
            throw new NotFoundException("Student profile not found for this user.");

        return MapToDto(student, student.User, student.College.CollegeName);
    }

    public async Task ApproveAsync(int id)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
            throw new NotFoundException($"Student with ID {id} not found.");

        if (student.CollegeId != _currentUserService.CollegeId)
            throw new UnauthorizedAccessException("You are not authorized to approve this student.");

        student.Status = AccountStatus.Approved;
        student.ApprovedAt = DateTime.UtcNow;
        student.User.AccountStatus = AccountStatus.Approved;
        student.User.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RejectAsync(int id)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
            throw new NotFoundException($"Student with ID {id} not found.");

        if (student.CollegeId != _currentUserService.CollegeId)
            throw new UnauthorizedAccessException("You are not authorized to reject this student.");

        student.Status = AccountStatus.Rejected;
        student.User.AccountStatus = AccountStatus.Rejected;
        student.User.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private static StudentResponseDto MapToDto(Student student, ApplicationUser user, string collegeName)
    {
        return new StudentResponseDto
        {
            StudentId = student.StudentId,
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            RegisterNumber = student.RegisterNumber,
            Department = student.Department,
            Degree = student.Degree,
            GraduationYear = student.GraduationYear,
            CGPA = student.CGPA,
            ActiveBacklogs = student.ActiveBacklogs,
            ResumeUrl = student.ResumeUrl,
            AccountStatus = user.AccountStatus.ToString(),
            CollegeId = student.CollegeId,
            CollegeName = collegeName,
            ApprovedAt = student.ApprovedAt
        };
    }
}