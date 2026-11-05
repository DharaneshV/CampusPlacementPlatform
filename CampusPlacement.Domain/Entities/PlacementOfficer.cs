using CampusPlacement.Domain.Enums;

namespace CampusPlacement.Domain.Entities;

public class PlacementOfficer
{
    public int PlacementOfficerId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int CollegeId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public AccountStatus Status { get; set; } = AccountStatus.Pending;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Navigation Properties
    public ApplicationUser User { get; set; } = null!;
    public College College { get; set; } = null!;
    public ApplicationUser? ApprovedByUser { get; set; }
    public ICollection<Student> EnrolledStudents { get; set; } = new List<Student>();
}