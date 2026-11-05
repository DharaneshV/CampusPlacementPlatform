namespace CampusPlacement.Domain.Entities;

public class JobEligibleGraduationYear
{
    public int JobEligibleGraduationYearId { get; set; }
    public int JobId { get; set; }
    public int GraduationYear { get; set; }

    // Navigation Property
    public JobOpening JobOpening { get; set; } = null!;
}