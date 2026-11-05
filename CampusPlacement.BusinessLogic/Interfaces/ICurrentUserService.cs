namespace CampusPlacement.BusinessLogic.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Role { get; }
    int? CollegeId { get; }
    int? CompanyId { get; }
    int? ProfileId { get; }
    bool IsAuthenticated { get; }
}