using CampusPlacement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacement.DataAccess.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.StudentId);

        builder.Property(s => s.RegisterNumber)
            .IsRequired()
            .HasMaxLength(50);

        // Section 5.7 & 6.1: RegisterNumber must be unique within a college
        builder.HasIndex(s => new { s.CollegeId, s.RegisterNumber })
            .IsUnique();

        builder.Property(s => s.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Degree)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.CGPA)
            .HasPrecision(4, 2)
            .IsRequired();

        builder.Property(s => s.ResumeUrl)
            .HasMaxLength(500);

        // 1:1 with ApplicationUser
        builder.HasOne(s => s.User)
            .WithOne(u => u.Student)
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1:M with College
        builder.HasOne(s => s.College)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1:M with PlacementOfficer
        builder.HasOne(s => s.PlacementOfficer)
            .WithMany(po => po.EnrolledStudents)
            .HasForeignKey(s => s.PlacementOfficerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}