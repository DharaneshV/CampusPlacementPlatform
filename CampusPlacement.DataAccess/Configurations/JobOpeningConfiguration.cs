using CampusPlacement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacement.DataAccess.Configurations;

public class JobOpeningConfiguration : IEntityTypeConfiguration<JobOpening>
{
    public void Configure(EntityTypeBuilder<JobOpening> builder)
    {
        builder.HasKey(j => j.JobId);

        builder.Property(j => j.JobTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.JobDescription)
            .IsRequired();

        builder.Property(j => j.JobType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.SalaryPackage)
            .HasPrecision(12, 2);

        builder.Property(j => j.MinimumCGPA)
            .HasPrecision(4, 2);

        builder.HasOne(j => j.Company)
            .WithMany(c => c.JobOpenings)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.CreatedByHR)
            .WithMany(hr => hr.CreatedJobs)
            .HasForeignKey(j => j.CreatedByHRId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class JobTargetCollegeConfiguration : IEntityTypeConfiguration<JobTargetCollege>
{
    public void Configure(EntityTypeBuilder<JobTargetCollege> builder)
    {
        builder.HasKey(jtc => jtc.JobTargetCollegeId);

        builder.HasIndex(jtc => new { jtc.JobId, jtc.CollegeId })
            .IsUnique();

        builder.HasOne(jtc => jtc.JobOpening)
            .WithMany(j => j.TargetColleges)
            .HasForeignKey(jtc => jtc.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(jtc => jtc.College)
            .WithMany(c => c.TargetedJobs)
            .HasForeignKey(jtc => jtc.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class JobEligibleDepartmentConfiguration : IEntityTypeConfiguration<JobEligibleDepartment>
{
    public void Configure(EntityTypeBuilder<JobEligibleDepartment> builder)
    {
        builder.HasKey(d => d.JobEligibleDepartmentId);

        builder.Property(d => d.DepartmentName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(d => new { d.JobId, d.DepartmentName })
            .IsUnique();

        builder.HasOne(d => d.JobOpening)
            .WithMany(j => j.EligibleDepartments)
            .HasForeignKey(d => d.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class JobEligibleGraduationYearConfiguration : IEntityTypeConfiguration<JobEligibleGraduationYear>
{
    public void Configure(EntityTypeBuilder<JobEligibleGraduationYear> builder)
    {
        builder.HasKey(y => y.JobEligibleGraduationYearId);

        builder.HasIndex(y => new { y.JobId, y.GraduationYear })
            .IsUnique();

        builder.HasOne(y => y.JobOpening)
            .WithMany(j => j.EligibleGraduationYears)
            .HasForeignKey(y => y.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}