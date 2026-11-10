namespace CampusPlacement.BusinessLogic.DTOs.Students;

public class BulkEnrollmentResultDto
{
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}