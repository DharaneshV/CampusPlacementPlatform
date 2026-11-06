using CampusPlacement.BusinessLogic.DTOs;
using CampusPlacement.BusinessLogic.DTOs.Companies;
using CampusPlacement.Domain.Enums;

namespace CampusPlacement.BusinessLogic.Interfaces;

public interface ICompanyService
{
    Task<CompanyResponseDto> RegisterAsync(RegisterCompanyDto dto);
    Task<IEnumerable<CompanyResponseDto>> GetAllAsync(OrganizationStatus? status = null);
    Task<CompanyResponseDto> GetByIdAsync(int id);
    Task UpdateStatusAsync(int id, OrganizationStatus status);
}