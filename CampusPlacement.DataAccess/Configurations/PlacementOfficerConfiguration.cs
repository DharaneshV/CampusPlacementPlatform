using CampusPlacement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacement.DataAccess.Configurations;

public class PlacementOfficerConfiguration : IEntityTypeConfiguration<PlacementOfficer>
{
    public void Configure(EntityTypeBuilder<PlacementOfficer> builder)
    {
        builder.HasKey(po => po.PlacementOfficerId);

        builder.Property(po => po.Designation)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(po => po.Status)
            .IsRequired();

        // 1:1 with ApplicationUser
        builder.HasOne(po => po.User)
            .WithOne(u => u.PlacementOfficer)
            .HasForeignKey<PlacementOfficer>(po => po.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1:M with College
        builder.HasOne(po => po.College)
            .WithMany(c => c.PlacementOfficers)
            .HasForeignKey(po => po.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional FK to SuperAdmin who approved the officer
        builder.HasOne(po => po.ApprovedByUser)
            .WithMany()
            .HasForeignKey(po => po.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}