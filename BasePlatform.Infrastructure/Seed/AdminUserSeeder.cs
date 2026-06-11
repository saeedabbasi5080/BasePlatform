using BasePlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BasePlatform.Infrastructure.Seed;

public static class AdminUserSeeder
{
    public static async Task SeedAsync(
        UserManager<AppUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var adminEmail = configuration["Seed:AdminEmail"] ?? "admin@baseplatform.com";
        var adminPassword = configuration["Seed:AdminPassword"] ?? "Admin@12345";

        var existingUser = await userManager.FindByEmailAsync(adminEmail);
        if (existingUser is not null)
            return;

        var adminUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = adminEmail,
            NormalizedUserName = adminEmail.ToUpperInvariant(),
            Email = adminEmail,
            NormalizedEmail = adminEmail.ToUpperInvariant(),
            FirstName = "Super",
            LastName = "Admin",
            DisplayName = "Super Admin",
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (!createResult.Succeeded)
        {
            logger.LogError("Failed to seed admin user: {Errors}",
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return;
        }

        var roleResult = await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
        if (roleResult.Succeeded)
            logger.LogInformation("Admin user '{Email}' seeded and assigned SuperAdmin role.", adminEmail);
        else
            logger.LogError("Failed to assign SuperAdmin role to admin user: {Errors}",
                string.Join(", ", roleResult.Errors.Select(e => e.Description)));
    }
}