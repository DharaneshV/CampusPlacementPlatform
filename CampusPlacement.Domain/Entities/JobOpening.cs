using CampusPlacement.Domain.Enums;

namespace CampusPlacement.Domain.Entities;

public class JobOpening
{
    public int JobId { get; set; }
    public int CompanyId { get; set; }
    public int CreatedByHRId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal? SalaryPackage { get; set; }
    public decimal? MinimumCGPA { get; set; }
    public int? MaximumBacklogs { get; set; }
    public DateTime ApplicationDeadline { get; set; }
    public bool IsOpenToAllColleges { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Company Company { get; set; } = null!;
    public CompanyHR CreatedByHR { get; set; } = null!;
    public ICollection<JobTargetCollege> TargetColleges { get; set; } = new List<JobTargetCollege>();
    public ICollection<JobEligibleDepartment> EligibleDepartments { get; set; } = new List<JobEligibleDepartment>();
    public ICollection<JobEligibleGraduationYear> EligibleGraduationYears { get; set; } = new List<JobEligibleGraduationYear>();
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    public ICollection<RecruitmentDrive> RecruitmentDrives { get; set; } = new List<RecruitmentDrive>();
    public ICollection<InterviewRound> InterviewRounds { get; set; } = new List<InterviewRound>();
}