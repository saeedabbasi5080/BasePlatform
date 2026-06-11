using BasePlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BasePlatform.Infrastructure.Seed;

public static class RoleSeeder
{
    private static readonly string[] Roles = ["SuperAdmin", "Admin", "User"];

    public static async Task SeedAsync(
        RoleManager<AppRole> roleManager,
        ILogger logger)
    {
        foreach (var roleName in Roles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpperInvariant(),
                Description = roleName switch
                {
                    "SuperAdmin" => "Full access to everything",
                    "Admin" => "User management, role management, content management",
                    "User" => "Basic authenticated user",
                    _ => string.Empty
                }
            };

            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
                logger.LogInformation("Role '{RoleName}' seeded successfully.", roleName);
            else
                logger.LogError("Failed to seed role '{RoleName}': {Errors}",
                    roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}