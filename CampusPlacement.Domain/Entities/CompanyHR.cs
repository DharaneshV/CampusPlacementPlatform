namespace CampusPlacement.Domain.Entities;

public class CompanyHR
{
    public int CompanyHRId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string? Designation { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Navigation Properties
    public ApplicationUser User { get; set; } = null!;
    public Company Company { get; set; } = null!;
    public ApplicationUser? ApprovedByUser { get; set; }
    public ICollection<JobOpening> CreatedJobs { get; set; } = new List<JobOpening>();
}