namespace CampusPlacement.BusinessLogic.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int? ProfileId { get; set; }
    public int? CollegeId { get; set; }
    public int? CompanyId { get; set; }
}