using CampusPlacement.Domain.Constants;
using CampusPlacement.Domain.Entities;
using CampusPlacement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampusPlacement.DataAccess.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Seed Roles
        string[] roles = { RoleConstants.SuperAdmin, RoleConstants.PlacementOfficer, RoleConstants.CompanyHR, RoleConstants.Student };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed SuperAdmin User
        var adminEmail = configuration["SuperAdminSeed:Email"];
        var adminPassword = configuration["SuperAdminSeed:Password"];
        var adminName = configuration["SuperAdminSeed:FullName"];

        if (adminEmail != null && await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = adminName ?? "Platform Super Admin",
                AccountStatus = AccountStatus.Approved,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword!);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, RoleConstants.SuperAdmin);
            }
        }
    }
}