using CampusPlacement.BusinessLogic.DTOs.Auth;

namespace CampusPlacement.BusinessLogic.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto request);
}