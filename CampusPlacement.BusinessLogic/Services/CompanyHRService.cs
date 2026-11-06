using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs;
using CampusPlacement.BusinessLogic.DTOs.Companies;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.DataAccess.Context;
using CampusPlacement.Domain.Constants;
using CampusPlacement.Domain.Entities;
using CampusPlacement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacement.BusinessLogic.Services;

public class CompanyHRService : ICompanyHRService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CompanyHRService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<CompanyHRResponseDto> RegisterAsync(RegisterCompanyHRDto dto)
    {
        var company = await _context.Companies.FindAsync(dto.CompanyId);
        if (company == null || company.Status != OrganizationStatus.Approved)
            throw new ConflictException("Selected company is invalid or not yet approved.");

        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            throw new ConflictException("An account with this email already exists.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                AccountStatus = AccountStatus.Pending
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, RoleConstants.CompanyHR);

            var hr = new CompanyHR
            {
                UserId = user.Id,
                CompanyId = dto.CompanyId,
                Designation = dto.Designation
            };

            _context.CompanyHRs.Add(hr);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return MapToDto(hr, user, company);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<CompanyHRResponseDto>> GetAllAsync()
    {
        var hrs = await _context.CompanyHRs
            .Include(hr => hr.User)
            .Include(hr => hr.Company)
            .AsNoTracking()
            .ToListAsync();

        return hrs.Select(hr => MapToDto(hr, hr.User, hr.Company));
    }

    public async Task<IEnumerable<CompanyHRResponseDto>> GetPendingAsync()
    {
        var hrs = await _context.CompanyHRs
            .Include(hr => hr.User)
            .Include(hr => hr.Company)
            .Where(hr => hr.User.AccountStatus == AccountStatus.Pending)
            .AsNoTracking()
            .ToListAsync();

        return hrs.Select(hr => MapToDto(hr, hr.User, hr.Company));
    }

    public async Task<CompanyHRResponseDto> GetByIdAsync(int id)
    {
        var hr = await _context.CompanyHRs
            .Include(hr => hr.User)
            .Include(hr => hr.Company)
            .AsNoTracking()
            .FirstOrDefaultAsync(hr => hr.CompanyHRId == id);

        if (hr == null)
            throw new NotFoundException($"HR profile with ID {id} not found.");

        return MapToDto(hr, hr.User, hr.Company);
    }

    public async Task<CompanyHRResponseDto> GetByUserIdAsync(string userId)
    {
        var hr = await _context.CompanyHRs
            .Include(hr => hr.User)
            .Include(hr => hr.Company)
            .AsNoTracking()
            .FirstOrDefaultAsync(hr => hr.UserId == userId);

        if (hr == null)
            throw new NotFoundException("HR profile not found for this user.");

        return MapToDto(hr, hr.User, hr.Company);
    }

    public async Task ApproveAsync(int id, string approvedByUserId)
    {
        var hr = await _context.CompanyHRs
            .Include(hr => hr.User)
            .Include(hr => hr.Company)
            .FirstOrDefaultAsync(h => h.CompanyHRId == id);

        if (hr == null)
            throw new NotFoundException($"HR profile with ID {id} not found.");

        if (hr.Company.Status != OrganizationStatus.Approved)
            throw new ConflictException("Cannot approve HR. Parent company is not approved.");

        hr.ApprovedBy = approvedByUserId;
        hr.ApprovedAt = DateTime.UtcNow;
        hr.User.AccountStatus = AccountStatus.Approved;
        hr.User.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RejectAsync(int id)
    {
        var hr = await _context.CompanyHRs
            .Include(hr => hr.User)
            .FirstOrDefaultAsync(h => h.CompanyHRId == id);

        if (hr == null)
            throw new NotFoundException($"HR profile with ID {id} not found.");

        hr.User.AccountStatus = AccountStatus.Rejected;
        hr.User.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private static CompanyHRResponseDto MapToDto(CompanyHR hr, ApplicationUser user, Company company)
    {
        return new CompanyHRResponseDto
        {
            CompanyHRId = hr.CompanyHRId,
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            CompanyId = hr.CompanyId,
            CompanyName = company.CompanyName,
            Designation = hr.Designation,
            AccountStatus = user.AccountStatus.ToString(),
            ApprovedAt = hr.ApprovedAt
        };
    }
}