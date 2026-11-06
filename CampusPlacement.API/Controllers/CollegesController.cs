using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.Colleges;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.Domain.Constants;
using CampusPlacement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollegesController : ControllerBase
{
    private readonly ICollegeService _collegeService;
    private readonly ICurrentUserService _currentUserService;

    public CollegesController(ICollegeService collegeService, ICurrentUserService currentUserService)
    {
        _collegeService = collegeService;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCollege([FromBody] RegisterCollegeDto dto)
    {
        try
        {
            var result = await _collegeService.RegisterCollegeAsync(dto);
            return CreatedAtAction(nameof(GetCollegeById), new { id = result.CollegeId }, result);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> GetAllColleges([FromQuery] OrganizationStatus? status)
    {
        var colleges = await _collegeService.GetAllCollegesAsync(status);
        return Ok(colleges);
    }

    [HttpGet("pending")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> GetPendingColleges()
    {
        var colleges = await _collegeService.GetAllCollegesAsync(OrganizationStatus.Pending);
        return Ok(colleges);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{RoleConstants.SuperAdmin},{RoleConstants.PlacementOfficer}")]
    public async Task<IActionResult> GetCollegeById(int id)
    {
        // Business Rule: Placement Officers can only view their own college
        if (_currentUserService.Role == RoleConstants.PlacementOfficer && _currentUserService.CollegeId != id)
        {
            return Forbid();
        }

        try
        {
            var college = await _collegeService.GetCollegeByIdAsync(id);
            return Ok(college);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> ApproveCollege(int id)
    {
        try
        {
            await _collegeService.UpdateCollegeStatusAsync(id, OrganizationStatus.Approved);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public async Task<IActionResult> RejectCollege(int id)
    {
        try
        {
            await _collegeService.UpdateCollegeStatusAsync(id, OrganizationStatus.Rejected);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}