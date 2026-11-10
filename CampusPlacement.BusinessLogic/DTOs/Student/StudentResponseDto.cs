namespace CampusPlacement.BusinessLogic.DTOs.Students;

public class StudentResponseDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string RegisterNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public decimal CGPA { get; set; }
    public int ActiveBacklogs { get; set; }
    public string? ResumeUrl { get; set; }
    public string AccountStatus { get; set; } = string.Empty;
    public int CollegeId { get; set; }
    public string CollegeName { get; set; } = string.Empty;
    public DateTime? ApprovedAt { get; set; }
}