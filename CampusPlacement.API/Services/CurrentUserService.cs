using CampusPlacement.BusinessLogic.Interfaces;
using System.Security.Claims;

namespace CampusPlacement.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
    public bool IsAuthenticated => UserId != null;

    public int? CollegeId
    {
        get
        {
            var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue("CollegeId");
            return string.IsNullOrEmpty(val) ? null : int.Parse(val);
        }
    }

    public int? CompanyId
    {
        get
        {
            var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue("CompanyId");
            return string.IsNullOrEmpty(val) ? null : int.Parse(val);
        }
    }

    public int? ProfileId
    {
        get
        {
            var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue("ProfileId");
            return string.IsNullOrEmpty(val) ? null : int.Parse(val);
        }
    }
}