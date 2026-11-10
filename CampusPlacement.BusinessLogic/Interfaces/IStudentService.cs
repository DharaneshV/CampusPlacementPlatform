using CampusPlacement.BusinessLogic.DTOs.Students;
using Microsoft.AspNetCore.Http;

namespace CampusPlacement.BusinessLogic.Interfaces;

public interface IStudentService
{
    Task<StudentResponseDto> EnrollAsync(EnrollStudentDto dto);
    Task<BulkEnrollmentResultDto> BulkEnrollAsync(IFormFile file);
    Task<IEnumerable<StudentResponseDto>> GetAllAsync();
    Task<IEnumerable<StudentResponseDto>> GetPendingAsync();
    Task<StudentResponseDto> GetByIdAsync(int id);
    Task<StudentResponseDto> GetByUserIdAsync(string userId);
    Task ApproveAsync(int id);
    Task RejectAsync(int id);
}