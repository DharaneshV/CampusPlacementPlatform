using CampusPlacement.BusinessLogic.DTOs.Colleges;
using CampusPlacement.Domain.Enums;

namespace CampusPlacement.BusinessLogic.Interfaces;

public interface ICollegeService
{
    Task<CollegeResponseDto> RegisterCollegeAsync(RegisterCollegeDto dto);
    Task<IEnumerable<CollegeResponseDto>> GetAllCollegesAsync(OrganizationStatus? status = null);
    Task<CollegeResponseDto> GetCollegeByIdAsync(int id);
    Task UpdateCollegeStatusAsync(int id, OrganizationStatus status);
}