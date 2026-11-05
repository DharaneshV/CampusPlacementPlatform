using CampusPlacement.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CampusPlacement.DataAccess.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<College> Colleges => Set<College>();
    public DbSet<PlacementOfficer> PlacementOfficers => Set<PlacementOfficer>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyHR> CompanyHRs => Set<CompanyHR>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<JobOpening> JobOpenings => Set<JobOpening>();
    public DbSet<JobTargetCollege> JobTargetColleges => Set<JobTargetCollege>();
    public DbSet<JobEligibleDepartment> JobEligibleDepartments => Set<JobEligibleDepartment>();
    public DbSet<JobEligibleGraduationYear> JobEligibleGraduationYears => Set<JobEligibleGraduationYear>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<RecruitmentDrive> RecruitmentDrives => Set<RecruitmentDrive>();
    public DbSet<InterviewRound> InterviewRounds => Set<InterviewRound>();
    public DbSet<InterviewSchedule> InterviewSchedules => Set<InterviewSchedule>();
    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories => Set<ApplicationStatusHistory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Automatically loads all IEntityTypeConfiguration<T> classes in this project
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}