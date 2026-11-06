namespace CampusPlacement.BusinessLogic.DTOs.Companies;

public class CompanyHRResponseDto
{
    public int CompanyHRId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public string AccountStatus { get; set; } = string.Empty;
    public DateTime? ApprovedAt { get; set; }
}