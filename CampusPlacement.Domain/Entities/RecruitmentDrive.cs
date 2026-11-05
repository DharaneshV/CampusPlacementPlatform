using CampusPlacement.Domain.Enums;

namespace CampusPlacement.Domain.Entities;

public class RecruitmentDrive
{
    public int DriveId { get; set; }
    public int JobId { get; set; }
    public int CollegeId { get; set; }
    public DateTime DriveDate { get; set; }
    public string? Venue { get; set; }
    public string? Description { get; set; }
    public DriveStatus Status { get; set; } = DriveStatus.Scheduled;

    // Navigation Properties
    public JobOpening JobOpening { get; set; } = null!;
    public College College { get; set; } = null!;
}