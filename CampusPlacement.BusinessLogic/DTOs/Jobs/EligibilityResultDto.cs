namespace CampusPlacement.BusinessLogic.DTOs.Jobs;

public class EligibilityResultDto
{
    public bool IsEligible { get; set; }
    public List<string> Reasons { get; set; } = new();
}