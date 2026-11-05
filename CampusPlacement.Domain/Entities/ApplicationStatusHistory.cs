using CampusPlacement.Domain.Enums;

namespace CampusPlacement.Domain.Entities;

public class ApplicationStatusHistory
{
    public int HistoryId { get; set; }
    public int ApplicationId { get; set; }
    public ApplicationStatus? PreviousStatus { get; set; }
    public ApplicationStatus NewStatus { get; set; }
    public string UpdatedByUserId { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public JobApplication JobApplication { get; set; } = null!;
    public ApplicationUser UpdatedByUser { get; set; } = null!;
}