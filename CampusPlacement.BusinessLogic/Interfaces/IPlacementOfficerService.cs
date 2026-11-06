using CampusPlacement.BusinessLogic.DTOs.PlacementOfficers;

namespace CampusPlacement.BusinessLogic.Interfaces;

public interface IPlacementOfficerService
{
    Task<PlacementOfficerResponseDto> RegisterAsync(RegisterPlacementOfficerDto dto);
    Task<IEnumerable<PlacementOfficerResponseDto>> GetAllAsync();
    Task<IEnumerable<PlacementOfficerResponseDto>> GetPendingAsync();
    Task<PlacementOfficerResponseDto> GetByIdAsync(int id);
    Task<PlacementOfficerResponseDto> GetByUserIdAsync(string userId);
    Task ApproveAsync(int id, string approvedByUserId);
    Task RejectAsync(int id);
}