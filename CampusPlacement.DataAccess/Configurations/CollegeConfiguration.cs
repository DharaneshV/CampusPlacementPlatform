using CampusPlacement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacement.DataAccess.Configurations;

public class CollegeConfiguration : IEntityTypeConfiguration<College>
{
    public void Configure(EntityTypeBuilder<College> builder)
    {
        builder.HasKey(c => c.CollegeId);

        builder.Property(c => c.CollegeName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.CollegeCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(c => c.CollegeCode)
            .IsUnique();

        builder.Property(c => c.UniversityName)
            .HasMaxLength(200);

        builder.Property(c => c.OfficialEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Website)
            .HasMaxLength(255);

        builder.Property(c => c.Status)
            .IsRequired();
    }
}