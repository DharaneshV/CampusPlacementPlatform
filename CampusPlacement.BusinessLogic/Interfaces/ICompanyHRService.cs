using CampusPlacement.BusinessLogic.DTOs;
using CampusPlacement.BusinessLogic.DTOs.Companies;

namespace CampusPlacement.BusinessLogic.Interfaces;

public interface ICompanyHRService
{
    Task<CompanyHRResponseDto> RegisterAsync(RegisterCompanyHRDto dto);
    Task<IEnumerable<CompanyHRResponseDto>> GetAllAsync();
    Task<IEnumerable<CompanyHRResponseDto>> GetPendingAsync();
    Task<CompanyHRResponseDto> GetByIdAsync(int id);
    Task<CompanyHRResponseDto> GetByUserIdAsync(string userId);
    Task ApproveAsync(int id, string approvedByUserId);
    Task RejectAsync(int id);
}