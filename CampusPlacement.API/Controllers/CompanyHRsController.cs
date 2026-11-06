using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.Companies;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacement.API.Controllers;

[ApiController]
[Route("api/company-hrs")]
public class CompanyHRsController : ControllerBase
{
    private readonly ICompanyHRService _companyHRService;
    private readonly ICurrentUserService _currentUserService;

    public CompanyHRsController(ICompanyHRService companyHRService, ICurrentUserService currentUserService)
    {
        _companyHRService = companyHRService;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCompanyHRDto dto)
    {
        try
        {
            var result = await _companyHRService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.CompanyHRId }, result);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> GetAll()
    {
        var hrs = await _companyHRService.GetAllAsync();
        return Ok(hrs);
    }

    [HttpGet("pending")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> GetPending()
    {
        var hrs = await _companyHRService.GetPendingAsync();
        return Ok(hrs);
    }

    [HttpGet("me")]
    [Authorize(Roles = RoleConstants.CompanyHR)]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var profile = await _companyHRService.GetByUserIdAsync(_currentUserService.UserId!);
            return Ok(profile);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{RoleConstants.SuperAdmin},{RoleConstants.CompanyHR}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (_currentUserService.Role == RoleConstants.CompanyHR && _currentUserService.ProfileId != id)
        {
            return Forbid();
        }

        try
        {
            var hr = await _companyHRService.GetByIdAsync(id);
            return Ok(hr);
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
            await _companyHRService.ApproveAsync(id, _currentUserService.UserId!);
            return NoContent();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
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
            await _companyHRService.RejectAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}