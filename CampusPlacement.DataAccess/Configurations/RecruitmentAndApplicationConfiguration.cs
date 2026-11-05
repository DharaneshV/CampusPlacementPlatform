using CampusPlacement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacement.DataAccess.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.HasKey(ja => ja.ApplicationId);

        // Section 5.10 & 6.1: Prevent duplicate job applications
        builder.HasIndex(ja => new { ja.StudentId, ja.JobId })
            .IsUnique();

        builder.Property(ja => ja.ResumeUrl)
            .HasMaxLength(500);

        builder.HasOne(ja => ja.JobOpening)
            .WithMany(j => j.Applications)
            .HasForeignKey(ja => ja.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ja => ja.Student)
            .WithMany(s => s.JobApplications)
            .HasForeignKey(ja => ja.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RecruitmentDriveConfiguration : IEntityTypeConfiguration<RecruitmentDrive>
{
    public void Configure(EntityTypeBuilder<RecruitmentDrive> builder)
    {
        builder.HasKey(rd => rd.DriveId);

        builder.Property(rd => rd.Venue)
            .HasMaxLength(255);

        builder.HasOne(rd => rd.JobOpening)
            .WithMany(j => j.RecruitmentDrives)
            .HasForeignKey(rd => rd.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rd => rd.College)
            .WithMany(c => c.RecruitmentDrives)
            .HasForeignKey(rd => rd.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InterviewRoundConfiguration : IEntityTypeConfiguration<InterviewRound>
{
    public void Configure(EntityTypeBuilder<InterviewRound> builder)
    {
        builder.HasKey(ir => ir.InterviewRoundId);

        builder.Property(ir => ir.RoundName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(ir => new { ir.JobId, ir.RoundNumber })
            .IsUnique();

        builder.HasOne(ir => ir.JobOpening)
            .WithMany(j => j.InterviewRounds)
            .HasForeignKey(ir => ir.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class InterviewScheduleConfiguration : IEntityTypeConfiguration<InterviewSchedule>
{
    public void Configure(EntityTypeBuilder<InterviewSchedule> builder)
    {
        builder.HasKey(isc => isc.InterviewScheduleId);

        builder.Property(isc => isc.MeetingLink)
            .HasMaxLength(500);

        builder.Property(isc => isc.Venue)
            .HasMaxLength(255);

        builder.HasOne(isc => isc.JobApplication)
            .WithMany(ja => ja.InterviewSchedules)
            .HasForeignKey(isc => isc.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(isc => isc.InterviewRound)
            .WithMany(ir => ir.InterviewSchedules)
            .HasForeignKey(isc => isc.InterviewRoundId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ApplicationStatusHistoryConfiguration : IEntityTypeConfiguration<ApplicationStatusHistory>
{
    public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
    {
        builder.HasKey(h => h.HistoryId);

        builder.Property(h => h.Remarks)
            .HasMaxLength(500);

        builder.HasOne(h => h.JobApplication)
            .WithMany(ja => ja.StatusHistories)
            .HasForeignKey(h => h.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.UpdatedByUser)
            .WithMany()
            .HasForeignKey(h => h.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}