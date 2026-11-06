using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.PlacementOfficers;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.DataAccess.Context;
using CampusPlacement.Domain.Constants;
using CampusPlacement.Domain.Entities;
using CampusPlacement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacement.BusinessLogic.Services;

public class PlacementOfficerService : IPlacementOfficerService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PlacementOfficerService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<PlacementOfficerResponseDto> RegisterAsync(RegisterPlacementOfficerDto dto)
    {
        // 1. Validate College
        var college = await _context.Colleges.FindAsync(dto.CollegeId);
        if (college == null || college.Status != OrganizationStatus.Approved)
            throw new ConflictException("Selected college is invalid or not yet approved.");

        // 2. Validate Email uniqueness
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            throw new ConflictException("An account with this email already exists.");

        // 3. Execute Transaction
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

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, RoleConstants.PlacementOfficer);

            var officer = new PlacementOfficer
            {
                UserId = user.Id,
                CollegeId = dto.CollegeId,
                Designation = dto.Designation,
                Status = AccountStatus.Pending
            };

            _context.PlacementOfficers.Add(officer);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return MapToDto(officer, user, college);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<PlacementOfficerResponseDto>> GetAllAsync()
    {
        var officers = await _context.PlacementOfficers
            .Include(po => po.User)
            .Include(po => po.College)
            .AsNoTracking()
            .ToListAsync();

        return officers.Select(po => MapToDto(po, po.User, po.College));
    }

    public async Task<IEnumerable<PlacementOfficerResponseDto>> GetPendingAsync()
    {
        var officers = await _context.PlacementOfficers
            .Include(po => po.User)
            .Include(po => po.College)
            .Where(po => po.Status == AccountStatus.Pending)
            .AsNoTracking()
            .ToListAsync();

        return officers.Select(po => MapToDto(po, po.User, po.College));
    }

    public async Task<PlacementOfficerResponseDto> GetByIdAsync(int id)
    {
        var officer = await _context.PlacementOfficers
            .Include(po => po.User)
            .Include(po => po.College)
            .AsNoTracking()
            .FirstOrDefaultAsync(po => po.PlacementOfficerId == id);

        if (officer == null)
            throw new NotFoundException($"Placement officer with ID {id} not found.");

        return MapToDto(officer, officer.User, officer.College);
    }

    public async Task<PlacementOfficerResponseDto> GetByUserIdAsync(string userId)
    {
        var officer = await _context.PlacementOfficers
            .Include(po => po.User)
            .Include(po => po.College)
            .AsNoTracking()
            .FirstOrDefaultAsync(po => po.UserId == userId);

        if (officer == null)
            throw new NotFoundException("Placement officer profile not found for this user.");

        return MapToDto(officer, officer.User, officer.College);
    }

    public async Task ApproveAsync(int id, string approvedByUserId)
    {
        var officer = await _context.PlacementOfficers
            .Include(po => po.User)
            .FirstOrDefaultAsync(po => po.PlacementOfficerId == id);

        if (officer == null)
            throw new NotFoundException($"Placement officer with ID {id} not found.");

        officer.Status = AccountStatus.Approved;
        officer.ApprovedBy = approvedByUserId;
        officer.ApprovedAt = DateTime.UtcNow;
        officer.User.AccountStatus = AccountStatus.Approved;
        officer.User.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RejectAsync(int id)
    {
        var officer = await _context.PlacementOfficers
            .Include(po => po.User)
            .FirstOrDefaultAsync(po => po.PlacementOfficerId == id);

        if (officer == null)
            throw new NotFoundException($"Placement officer with ID {id} not found.");

        officer.Status = AccountStatus.Rejected;
        officer.User.AccountStatus = AccountStatus.Rejected;
        officer.User.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private static PlacementOfficerResponseDto MapToDto(PlacementOfficer po, ApplicationUser user, College college)
    {
        return new PlacementOfficerResponseDto
        {
            PlacementOfficerId = po.PlacementOfficerId,
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            CollegeId = po.CollegeId,
            CollegeName = college.CollegeName,
            Designation = po.Designation,
            Status = po.Status.ToString(),
            ApprovedAt = po.ApprovedAt
        };
    }
}