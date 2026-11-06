using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs;
using CampusPlacement.BusinessLogic.DTOs.Companies;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.DataAccess.Context;
using CampusPlacement.Domain.Entities;
using CampusPlacement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacement.BusinessLogic.Services;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _context;

    public CompanyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyResponseDto> RegisterAsync(RegisterCompanyDto dto)
    {
        if (await _context.Companies.AnyAsync(c => c.CompanyEmail == dto.CompanyEmail))
            throw new ConflictException("A company with this official email is already registered.");

        var company = new Company
        {
            CompanyName = dto.CompanyName,
            CompanyEmail = dto.CompanyEmail,
            Website = dto.Website,
            Industry = dto.Industry,
            Address = dto.Address,
            Description = dto.Description,
            Status = OrganizationStatus.Pending
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return MapToDto(company);
    }

    public async Task<IEnumerable<CompanyResponseDto>> GetAllAsync(OrganizationStatus? status = null)
    {
        var query = _context.Companies.AsNoTracking();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        var companies = await query.ToListAsync();
        return companies.Select(MapToDto);
    }

    public async Task<CompanyResponseDto> GetByIdAsync(int id)
    {
        var company = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.CompanyId == id);

        if (company == null)
            throw new NotFoundException($"Company with ID {id} not found.");

        return MapToDto(company);
    }

    public async Task UpdateStatusAsync(int id, OrganizationStatus status)
    {
        var company = await _context.Companies.FindAsync(id);

        if (company == null)
            throw new NotFoundException($"Company with ID {id} not found.");

        company.Status = status;
        await _context.SaveChangesAsync();
    }

    private static CompanyResponseDto MapToDto(Company company)
    {
        return new CompanyResponseDto
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            CompanyEmail = company.CompanyEmail,
            Website = company.Website,
            Industry = company.Industry,
            Address = company.Address,
            Description = company.Description,
            Status = company.Status.ToString(),
            CreatedAt = company.CreatedAt
        };
    }
}