using CampusPlacement.Domain.Enums;

namespace CampusPlacement.Domain.Entities;

public class Student
{
    public int StudentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int CollegeId { get; set; }
    public int PlacementOfficerId { get; set; }
    public string RegisterNumber { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public decimal CGPA { get; set; }
    public int ActiveBacklogs { get; set; }
    public string? ResumeUrl { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.Pending;
    public DateTime? ApprovedAt { get; set; }

    // Navigation Properties
    public ApplicationUser User { get; set; } = null!;
    public College College { get; set; } = null!;
    public PlacementOfficer PlacementOfficer { get; set; } = null!;
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}