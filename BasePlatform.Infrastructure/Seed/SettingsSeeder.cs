using BasePlatform.Domain.Entities;
using BasePlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BasePlatform.Infrastructure.Seed;

public static class SettingsSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        var settings = new List<(string Key, string Value, string Description, bool IsPublic)>
        {
            ("app.name",            "BasePlatform", "Application name",              true),
            ("app.version",         "1.0.0",        "Application version",           true),
            ("app.maintenance",     "false",         "Maintenance mode flag",         false),
            ("email.confirmation",  "true",          "Require email confirmation",    false),
            ("storage.provider",    "local",         "Active storage provider",       false),
            ("token.accessExpiry",  "15",            "JWT access token expiry (min)", false),
            ("token.refreshExpiry", "7",             "Refresh token expiry (days)",   false),
        };

        var existingKeys = await context.AppSettings
            .Select(s => s.Key)
            .ToListAsync();

        var existingSet = new HashSet<string>(existingKeys);

        foreach (var (key, value, description, isPublic) in settings)
        {
            if (existingSet.Contains(key))
                continue;

            context.AppSettings.Add(new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = value,
                Description = description,
                IsPublic = isPublic,
                UpdatedAt = DateTimeOffset.UtcNow,
                UpdatedByUserId = null
            });

            logger.LogInformation("Seeded setting: {Key}", key);
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true ||
                                            ex.InnerException?.Message.Contains("Duplicate") == true ||
                                            ex.InnerException?.Message.Contains("unique") == true)
        {
            logger.LogWarning("Some settings already seeded by another process. Skipping.");
            context.ChangeTracker.Clear();
        }
    }
}