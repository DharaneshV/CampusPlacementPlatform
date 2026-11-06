namespace CampusPlacement.BusinessLogic.DTOs.Companies;

public class RegisterCompanyHRDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string? Designation { get; set; }
}