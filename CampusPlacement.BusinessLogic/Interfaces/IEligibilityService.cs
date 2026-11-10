using CampusPlacement.BusinessLogic.DTOs.Jobs;

namespace CampusPlacement.BusinessLogic.Interfaces;

public interface IEligibilityService
{
    Task<EligibilityResultDto> EvaluateStudentEligibilityAsync(int studentId, int jobId);
}