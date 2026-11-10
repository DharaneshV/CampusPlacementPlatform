using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.Jobs;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly IEligibilityService _eligibilityService;
    private readonly ICurrentUserService _currentUserService;

    public JobsController(IJobService jobService, IEligibilityService eligibilityService, ICurrentUserService currentUserService)
    {
        _jobService = jobService;
        _eligibilityService = eligibilityService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.CompanyHR)]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobDto dto)
    {
        var result = await _jobService.CreateJobAsync(dto);
        return CreatedAtAction(nameof(GetJobById), new { id = result.JobId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobs()
    {
        var jobs = await _jobService.GetAllJobsAsync();
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobById(int id)
    {
        try
        {
            var job = await _jobService.GetJobByIdAsync(id);
            return Ok(job);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/publish")]
    [Authorize(Roles = RoleConstants.CompanyHR)]
    public async Task<IActionResult> PublishJob(int id)
    {
        try
        {
            await _jobService.PublishJobAsync(id);
            return NoContent();
        }
        catch (Exception ex) when (ex is NotFoundException || ex is UnauthorizedAccessException || ex is ConflictException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/close")]
    [Authorize(Roles = RoleConstants.CompanyHR)]
    public async Task<IActionResult> CloseJob(int id)
    {
        try
        {
            await _jobService.CloseJobAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleConstants.CompanyHR)]
    public async Task<IActionResult> DeleteDraftJob(int id)
    {
        try
        {
            await _jobService.DeleteDraftJobAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("eligible")]
    [Authorize(Roles = RoleConstants.Student)]
    public async Task<IActionResult> GetEligibleJobs()
    {
        var jobs = await _jobService.GetEligibleJobsForStudentAsync(_currentUserService.ProfileId!.Value);
        return Ok(jobs);
    }

    [HttpGet("{id}/eligibility")]
    [Authorize(Roles = RoleConstants.Student)]
    public async Task<IActionResult> CheckEligibility(int id)
    {
        try
        {
            var result = await _eligibilityService.EvaluateStudentEligibilityAsync(_currentUserService.ProfileId!.Value, id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}