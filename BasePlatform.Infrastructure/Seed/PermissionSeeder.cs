using BasePlatform.Domain.Entities;
using BasePlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BasePlatform.Infrastructure.Seed;

public static class PermissionSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        var permissions = new List<(string Name, string Group, string Description)>
        {
            ("users.view",         "Users",       "View users"),
            ("users.create",       "Users",       "Create users"),
            ("users.edit",         "Users",       "Edit users"),
            ("users.delete",       "Users",       "Delete users"),
            ("roles.view",         "Roles",       "View roles"),
            ("roles.create",       "Roles",       "Create roles"),
            ("roles.edit",         "Roles",       "Edit roles"),
            ("roles.delete",       "Roles",       "Delete roles"),
            ("roles.assign",       "Roles",       "Assign roles to users"),
            ("permissions.view",   "Permissions", "View permissions"),
            ("permissions.manage", "Permissions", "Manage permissions"),
            ("settings.view",      "Settings",    "View settings"),
            ("settings.update",    "Settings",    "Update settings"),
            ("files.upload",       "Files",       "Upload files"),
            ("files.delete",       "Files",       "Delete files"),
            ("audit.view",         "Audit",       "View audit logs"),
            ("admin.access",       "Admin",       "Access admin panel"),
        };

        // یکبار همه موجودها را بگیر
        var existingNames = await context.Permissions
            .Select(p => p.Name)
            .ToListAsync();

        var existingSet = new HashSet<string>(existingNames);

        foreach (var (name, group, description) in permissions)
        {
            if (existingSet.Contains(name))
                continue;

            context.Permissions.Add(new Permission
            {
                Id = Guid.NewGuid(),
                Name = name,
                Group = group,
                Description = description,
                CreatedAt = DateTimeOffset.UtcNow
            });

            logger.LogInformation("Seeded permission: {Permission}", name);
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true ||
                                            ex.InnerException?.Message.Contains("Duplicate") == true ||
                                            ex.InnerException?.Message.Contains("unique") == true)
        {
            // race condition بین Api و Admin — بی‌خطر است
            logger.LogWarning("Some permissions already seeded by another process. Skipping.");
            context.ChangeTracker.Clear();
        }
    }
}