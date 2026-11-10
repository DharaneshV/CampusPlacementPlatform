using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.Students;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ICurrentUserService _currentUserService;

    public StudentsController(IStudentService studentService, ICurrentUserService currentUserService)
    {
        _studentService = studentService;
        _currentUserService = currentUserService;
    }

    [HttpPost("enroll")]
    [Authorize(Roles = RoleConstants.PlacementOfficer)]
    public async Task<IActionResult> Enroll([FromBody] EnrollStudentDto dto)
    {
        try
        {
            var result = await _studentService.EnrollAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.StudentId }, result);
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

    [HttpPost("bulk-enroll")]
    [Authorize(Roles = RoleConstants.PlacementOfficer)]
    public async Task<IActionResult> BulkEnroll(IFormFile file)
    {
        try
        {
            var result = await _studentService.BulkEnrollAsync(file);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleConstants.SuperAdmin},{RoleConstants.PlacementOfficer}")]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAllAsync();
        return Ok(students);
    }

    [HttpGet("pending")]
    [Authorize(Roles = RoleConstants.PlacementOfficer)]
    public async Task<IActionResult> GetPending()
    {
        var students = await _studentService.GetPendingAsync();
        return Ok(students);
    }

    [HttpGet("me")]
    [Authorize(Roles = RoleConstants.Student)]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var profile = await _studentService.GetByUserIdAsync(_currentUserService.UserId!);
            return Ok(profile);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{RoleConstants.SuperAdmin},{RoleConstants.PlacementOfficer},{RoleConstants.Student}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (_currentUserService.Role == RoleConstants.Student && _currentUserService.ProfileId != id)
        {
            return Forbid();
        }

        try
        {
            var student = await _studentService.GetByIdAsync(id);
            return Ok(student);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = RoleConstants.PlacementOfficer)]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            await _studentService.ApproveAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = RoleConstants.PlacementOfficer)]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            await _studentService.RejectAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}