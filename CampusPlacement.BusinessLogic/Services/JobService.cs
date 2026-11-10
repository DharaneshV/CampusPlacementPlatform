using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.Jobs;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.DataAccess.Context;
using CampusPlacement.Domain.Constants;
using CampusPlacement.Domain.Entities;
using CampusPlacement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacement.BusinessLogic.Services;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEligibilityService _eligibilityService;

    public JobService(ApplicationDbContext context, ICurrentUserService currentUserService, IEligibilityService eligibilityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _eligibilityService = eligibilityService;
    }

    public async Task<JobResponseDto> CreateJobAsync(CreateJobDto dto)
    {
        var companyId = _currentUserService.CompanyId!.Value;
        var hrId = _currentUserService.ProfileId!.Value;

        var job = new JobOpening
        {
            CompanyId = companyId,
            CreatedByHRId = hrId,
            JobTitle = dto.JobTitle,
            JobDescription = dto.JobDescription,
            JobType = dto.JobType,
            Location = dto.Location,
            SalaryPackage = dto.SalaryPackage,
            MinimumCGPA = dto.MinimumCGPA,
            MaximumBacklogs = dto.MaximumBacklogs,
            ApplicationDeadline = dto.ApplicationDeadline,
            IsOpenToAllColleges = dto.IsOpenToAllColleges,
            Status = JobStatus.Draft,
            EligibleDepartments = dto.EligibleDepartments.Select(d => new JobEligibleDepartment { DepartmentName = d }).ToList(),
            EligibleGraduationYears = dto.EligibleGraduationYears.Select(y => new JobEligibleGraduationYear { GraduationYear = y }).ToList(),
            TargetColleges = dto.TargetCollegeIds.Select(cId => new JobTargetCollege { CollegeId = cId }).ToList()
        };

        _context.JobOpenings.Add(job);
        await _context.SaveChangesAsync();

        return await GetJobByIdAsync(job.JobId);
    }

    public async Task<IEnumerable<JobResponseDto>> GetAllJobsAsync()
    {
        var query = _context.JobOpenings
            .Include(j => j.Company)
            .Include(j => j.EligibleDepartments)
            .Include(j => j.EligibleGraduationYears)
            .Include(j => j.TargetColleges)
            .AsNoTracking();

        // Apply visibility rules based on Role
        if (_currentUserService.Role == RoleConstants.CompanyHR)
        {
            query = query.Where(j => j.CompanyId == _currentUserService.CompanyId);
        }
        else if (_currentUserService.Role == RoleConstants.PlacementOfficer || _currentUserService.Role == RoleConstants.Student)
        {
            var myCollegeId = _currentUserService.CollegeId;
            query = query.Where(j =>
                (j.Status == JobStatus.Published || j.Status == JobStatus.Closed) &&
                (j.IsOpenToAllColleges || j.TargetColleges.Any(tc => tc.CollegeId == myCollegeId)));
        }

        var jobs = await query.ToListAsync();
        return jobs.Select(MapToDto);
    }

    public async Task<IEnumerable<JobResponseDto>> GetEligibleJobsForStudentAsync(int studentId)
    {
        var allVisibleJobs = await GetAllJobsAsync();
        var eligibleJobs = new List<JobResponseDto>();

        foreach (var job in allVisibleJobs)
        {
            var eligibility = await _eligibilityService.EvaluateStudentEligibilityAsync(studentId, job.JobId);
            if (eligibility.IsEligible)
            {
                eligibleJobs.Add(job);
            }
        }

        return eligibleJobs;
    }

    public async Task<JobResponseDto> GetJobByIdAsync(int id)
    {
        var job = await _context.JobOpenings
            .Include(j => j.Company)
            .Include(j => j.EligibleDepartments)
            .Include(j => j.EligibleGraduationYears)
            .Include(j => j.TargetColleges)
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.JobId == id);

        if (job == null) throw new NotFoundException($"Job with ID {id} not found.");

        return MapToDto(job);
    }

    public async Task PublishJobAsync(int id)
    {
        var job = await _context.JobOpenings.Include(j => j.TargetColleges).FirstOrDefaultAsync(j => j.JobId == id);
        if (job == null) throw new NotFoundException($"Job with ID {id} not found.");

        if (job.CompanyId != _currentUserService.CompanyId) throw new UnauthorizedAccessException();

        if (job.ApplicationDeadline <= DateTime.UtcNow) throw new ConflictException("Cannot publish job with a past deadline.");
        if (!job.IsOpenToAllColleges && !job.TargetColleges.Any()) throw new ConflictException("Job must either be open to all colleges or have at least one target college.");

        job.Status = JobStatus.Published;
        await _context.SaveChangesAsync();
    }

    public async Task CloseJobAsync(int id)
    {
        var job = await _context.JobOpenings.FindAsync(id);
        if (job == null) throw new NotFoundException($"Job not found.");
        if (job.CompanyId != _currentUserService.CompanyId) throw new UnauthorizedAccessException();

        job.Status = JobStatus.Closed;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDraftJobAsync(int id)
    {
        var job = await _context.JobOpenings.FindAsync(id);
        if (job == null) throw new NotFoundException($"Job not found.");
        if (job.CompanyId != _currentUserService.CompanyId) throw new UnauthorizedAccessException();
        if (job.Status != JobStatus.Draft) throw new ConflictException("Only draft jobs can be deleted.");

        _context.JobOpenings.Remove(job);
        await _context.SaveChangesAsync();
    }

    private static JobResponseDto MapToDto(JobOpening j)
    {
        return new JobResponseDto
        {
            JobId = j.JobId,
            CompanyId = j.CompanyId,
            CompanyName = j.Company?.CompanyName ?? string.Empty,
            JobTitle = j.JobTitle,
            JobDescription = j.JobDescription,
            JobType = j.JobType,
            Location = j.Location,
            SalaryPackage = j.SalaryPackage,
            MinimumCGPA = j.MinimumCGPA,
            MaximumBacklogs = j.MaximumBacklogs,
            ApplicationDeadline = j.ApplicationDeadline,
            IsOpenToAllColleges = j.IsOpenToAllColleges,
            Status = j.Status.ToString(),
            CreatedAt = j.CreatedAt,
            EligibleDepartments = j.EligibleDepartments.Select(d => d.DepartmentName).ToList(),
            EligibleGraduationYears = j.EligibleGraduationYears.Select(y => y.GraduationYear).ToList(),
            TargetCollegeIds = j.TargetColleges.Select(tc => tc.CollegeId).ToList()
        };
    }
}