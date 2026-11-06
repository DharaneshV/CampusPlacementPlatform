using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.Companies;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.Domain.Constants;
using CampusPlacement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ICurrentUserService _currentUserService;

    public CompaniesController(ICompanyService companyService, ICurrentUserService currentUserService)
    {
        _companyService = companyService;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCompanyDto dto)
    {
        try
        {
            var result = await _companyService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.CompanyId }, result);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> GetAll([FromQuery] OrganizationStatus? status)
    {
        var companies = await _companyService.GetAllAsync(status);
        return Ok(companies);
    }

    [HttpGet("pending")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> GetPending()
    {
        var companies = await _companyService.GetAllAsync(OrganizationStatus.Pending);
        return Ok(companies);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{RoleConstants.SuperAdmin},{RoleConstants.CompanyHR}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (_currentUserService.Role == RoleConstants.CompanyHR && _currentUserService.CompanyId != id)
        {
            return Forbid();
        }

        try
        {
            var company = await _companyService.GetByIdAsync(id);
            return Ok(company);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            await _companyService.UpdateStatusAsync(id, OrganizationStatus.Approved);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            await _companyService.UpdateStatusAsync(id, OrganizationStatus.Rejected);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}