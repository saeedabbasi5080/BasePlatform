using BasePlatform.Domain.Entities;
using BasePlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BasePlatform.Infrastructure.Seed;

public static class SettingsSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var existingKeys = await context.AppSettings
            .AsNoTracking()
            .Select(s => s.Key)
            .ToListAsync(cancellationToken);

        var defaults = BuildDefaultSettings()
            .Where(s => !existingKeys.Contains(s.Key))
            .ToList();

        if (defaults.Count == 0)
        {
            logger.LogInformation("Settings already seeded.");
            return;
        }

        await context.AppSettings.AddRangeAsync(defaults, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("{Count} settings seeded successfully.", defaults.Count);
    }

    private static List<AppSetting> BuildDefaultSettings()
    {
        var now = DateTimeOffset.UtcNow;

        return
        [
            new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = "App.Name",
                Value = "BasePlatform",
                Description = "Application display name",
                IsPublic = true,
                UpdatedAt = now
            },
            new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = "App.Version",
                Value = "1.0.0",
                Description = "Current application version",
                IsPublic = true,
                UpdatedAt = now
            },
            new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = "App.MaintenanceMode",
                Value = "false",
                Description = "When true, API returns 503 for non-admin requests",
                IsPublic = false,
                UpdatedAt = now
            },
            new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = "Email.SupportAddress",
                Value = "support@example.com",
                Description = "Support email shown to users",
                IsPublic = true,
                UpdatedAt = now
            },
            new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = "Auth.MaxFailedLoginAttempts",
                Value = "5",
                Description = "Max failed login attempts before lockout",
                IsPublic = false,
                UpdatedAt = now
            }
        ];
    }
}