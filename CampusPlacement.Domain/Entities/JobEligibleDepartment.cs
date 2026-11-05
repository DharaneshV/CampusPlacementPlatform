namespace CampusPlacement.Domain.Entities;

public class JobEligibleDepartment
{
    public int JobEligibleDepartmentId { get; set; }
    public int JobId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;

    // Navigation Property
    public JobOpening JobOpening { get; set; } = null!;
}