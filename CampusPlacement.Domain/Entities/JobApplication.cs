using CampusPlacement.Domain.Enums;

namespace CampusPlacement.Domain.Entities;

public class JobApplication
{
    public int ApplicationId { get; set; }
    public int JobId { get; set; }
    public int StudentId { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
    public string? ResumeUrl { get; set; }
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public JobOpening JobOpening { get; set; } = null!;
    public Student Student { get; set; } = null!;
    public ICollection<InterviewSchedule> InterviewSchedules { get; set; } = new List<InterviewSchedule>();
    public ICollection<ApplicationStatusHistory> StatusHistories { get; set; } = new List<ApplicationStatusHistory>();
}