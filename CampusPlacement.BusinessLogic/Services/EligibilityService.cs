using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.Jobs;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.DataAccess.Context;
using CampusPlacement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacement.BusinessLogic.Services;

public class EligibilityService : IEligibilityService
{
    private readonly ApplicationDbContext _context;

    public EligibilityService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EligibilityResultDto> EvaluateStudentEligibilityAsync(int studentId, int jobId)
    {
        var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null) throw new NotFoundException("Student not found.");

        var job = await _context.JobOpenings
            .Include(j => j.TargetColleges)
            .Include(j => j.EligibleDepartments)
            .Include(j => j.EligibleGraduationYears)
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.JobId == jobId);

        if (job == null) throw new NotFoundException("Job not found.");

        var result = new EligibilityResultDto { IsEligible = true };

        if (job.Status != JobStatus.Published)
        {
            result.IsEligible = false;
            result.Reasons.Add("Job is not currently published.");
        }

        if (job.ApplicationDeadline < DateTime.UtcNow)
        {
            result.IsEligible = false;
            result.Reasons.Add("The application deadline has passed.");
        }

        if (!job.IsOpenToAllColleges && !job.TargetColleges.Any(tc => tc.CollegeId == student.CollegeId))
        {
            result.IsEligible = false;
            result.Reasons.Add("This job is not targeted to your college.");
        }

        if (job.MinimumCGPA.HasValue && student.CGPA < job.MinimumCGPA.Value)
        {
            result.IsEligible = false;
            result.Reasons.Add($"CGPA {student.CGPA} is below the required {job.MinimumCGPA.Value}.");
        }

        if (job.MaximumBacklogs.HasValue && student.ActiveBacklogs > job.MaximumBacklogs.Value)
        {
            result.IsEligible = false;
            result.Reasons.Add($"Active backlogs ({student.ActiveBacklogs}) exceed the maximum allowed ({job.MaximumBacklogs.Value}).");
        }

        if (job.EligibleDepartments.Any() && !job.EligibleDepartments.Any(d => d.DepartmentName == student.Department))
        {
            result.IsEligible = false;
            result.Reasons.Add($"Department {student.Department} is not eligible.");
        }

        if (job.EligibleGraduationYears.Any() && !job.EligibleGraduationYears.Any(y => y.GraduationYear == student.GraduationYear))
        {
            result.IsEligible = false;
            result.Reasons.Add($"Graduation year {student.GraduationYear} is not eligible.");
        }

        return result;
    }
}