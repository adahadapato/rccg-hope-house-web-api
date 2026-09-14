using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Infrastructure.Identity;

/// <summary>
/// Seeds initial roles and admin user into ASP.NET Core Identity.
/// Run once at application startup to ensure required authorization policies exist.
/// </summary>
public static class RoleSeeder
{
    /// <summary>
    /// Seeds default roles (Admin, ContentEditor, MediaManager, PrayerTeam) and a default admin user.
    /// Idempotent: safe to run on every startup; only creates missing data.
    /// </summary>
    /// <param name="roleManager">The RoleManager for role operations.</param>
    /// <param name="userManager">The UserManager for user operations.</param>
    /// <param name="logger">Logger for seeding progress and errors.</param>
    /// <param name="adminEmail">Email for the default admin account (from configuration).</param>
    /// <param name="adminPassword">Password for the default admin account (from configuration).</param>
    public static async Task SeedAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        string adminEmail,
        string adminPassword)
    {
        // ==================== Seed Roles ====================
        var roles = new[]
        {
            Roles.Admin,
            Roles.ContentEditor,
            Roles.MediaManager,
            Roles.PrayerTeam
        };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    logger.LogInformation("Created role: {RoleName}", roleName);
                else
                    logger.LogError("Failed to create role {RoleName}: {Errors}", roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ==================== Seed Default Admin User ====================
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                logger.LogInformation("Created default admin user: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Failed to create admin user {Email}: {Errors}", adminEmail, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            logger.LogInformation("Added existing user {Email} to Admin role", adminEmail);
        }
    }
}