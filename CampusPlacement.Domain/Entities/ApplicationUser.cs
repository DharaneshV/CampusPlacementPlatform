using CampusPlacement.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacement.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Refresh token support for JWT rotation
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // 1:1 Profile Navigation Properties
    public PlacementOfficer? PlacementOfficer { get; set; }
    public CompanyHR? CompanyHR { get; set; }
    public Student? Student { get; set; }
}