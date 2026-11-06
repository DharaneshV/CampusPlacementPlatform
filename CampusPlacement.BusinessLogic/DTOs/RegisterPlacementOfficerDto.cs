namespace CampusPlacement.BusinessLogic.DTOs.PlacementOfficers;

public class RegisterPlacementOfficerDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int CollegeId { get; set; }
    public string Designation { get; set; } = string.Empty;
}