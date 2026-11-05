using CampusPlacement.Domain.Enums;

namespace CampusPlacement.Domain.Entities;

public class Company
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string Industry { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public OrganizationStatus Status { get; set; } = OrganizationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ICollection<CompanyHR> CompanyHRs { get; set; } = new List<CompanyHR>();
    public ICollection<JobOpening> JobOpenings { get; set; } = new List<JobOpening>();
}