using CampusPlacement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacement.DataAccess.Configurations;

public class CompanyHRConfiguration : IEntityTypeConfiguration<CompanyHR>
{
    public void Configure(EntityTypeBuilder<CompanyHR> builder)
    {
        builder.HasKey(hr => hr.CompanyHRId);

        builder.Property(hr => hr.Designation)
            .HasMaxLength(100);

        // 1:1 with ApplicationUser
        builder.HasOne(hr => hr.User)
            .WithOne(u => u.CompanyHR)
            .HasForeignKey<CompanyHR>(hr => hr.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1:M with Company
        builder.HasOne(hr => hr.Company)
            .WithMany(c => c.CompanyHRs)
            .HasForeignKey(hr => hr.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional FK to SuperAdmin who approved the HR
        builder.HasOne(hr => hr.ApprovedByUser)
            .WithMany()
            .HasForeignKey(hr => hr.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}