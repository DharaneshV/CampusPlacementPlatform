using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.PlacementOfficers;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacement.API.Controllers;

[ApiController]
[Route("api/placement-officers")]
public class PlacementOfficersController : ControllerBase
{
    private readonly IPlacementOfficerService _placementOfficerService;
    private readonly ICurrentUserService _currentUserService;

    public PlacementOfficersController(IPlacementOfficerService placementOfficerService, ICurrentUserService currentUserService)
    {
        _placementOfficerService = placementOfficerService;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterPlacementOfficerDto dto)
    {
        try
        {
            var result = await _placementOfficerService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.PlacementOfficerId }, result);
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
        var officers = await _placementOfficerService.GetAllAsync();
        return Ok(officers);
    }

    [HttpGet("pending")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> GetPending()
    {
        var officers = await _placementOfficerService.GetPendingAsync();
        return Ok(officers);
    }

    [HttpGet("me")]
    [Authorize(Roles = RoleConstants.PlacementOfficer)]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var profile = await _placementOfficerService.GetByUserIdAsync(_currentUserService.UserId!);
            return Ok(profile);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{RoleConstants.SuperAdmin},{RoleConstants.PlacementOfficer}")]
    public async Task<IActionResult> GetById(int id)
    {
        // Business Rule: A placement officer can only view their own profile by ID
        if (_currentUserService.Role == RoleConstants.PlacementOfficer && _currentUserService.ProfileId != id)
        {
            return Forbid();
        }

        try
        {
            var officer = await _placementOfficerService.GetByIdAsync(id);
            return Ok(officer);
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
            await _placementOfficerService.ApproveAsync(id, _currentUserService.UserId!);
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
            await _placementOfficerService.RejectAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}