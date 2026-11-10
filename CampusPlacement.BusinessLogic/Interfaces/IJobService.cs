using CampusPlacement.BusinessLogic.DTOs.Jobs;

namespace CampusPlacement.BusinessLogic.Interfaces;

public interface IJobService
{
    Task<JobResponseDto> CreateJobAsync(CreateJobDto dto);
    Task<IEnumerable<JobResponseDto>> GetAllJobsAsync();
    Task<IEnumerable<JobResponseDto>> GetEligibleJobsForStudentAsync(int studentId);
    Task<JobResponseDto> GetJobByIdAsync(int id);
    Task PublishJobAsync(int id);
    Task CloseJobAsync(int id);
    Task DeleteDraftJobAsync(int id);
}