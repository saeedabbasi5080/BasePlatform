using BasePlatform.Domain.Constants;
using BasePlatform.Domain.Entities;
using BasePlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BasePlatform.Infrastructure.Seed;

public static class PermissionSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var existingPermissions = await context.Permissions
            .AsNoTracking()
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);

        var missingPermissions = BuildPermissions()
            .Where(x => !existingPermissions.Contains(x.Name))
            .ToList();

        if (missingPermissions.Count == 0)
        {
            logger.LogInformation("Permissions already seeded.");
            return;
        }

        await context.Permissions.AddRangeAsync(missingPermissions, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("{Count} permissions seeded successfully.", missingPermissions.Count);
    }

    private static List<Permission> BuildPermissions()
    {
        var now = DateTimeOffset.UtcNow;

        return
        [
            new Permission { Id = Guid.NewGuid(), Name = Permissions.UsersView,        Description = "View users",             Group = "Users",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.UsersCreate,      Description = "Create users",           Group = "Users",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.UsersEdit,        Description = "Edit users",             Group = "Users",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.UsersDelete,      Description = "Delete users",           Group = "Users",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.RolesView,        Description = "View roles",             Group = "Roles",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.RolesCreate,      Description = "Create roles",           Group = "Roles",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.RolesEdit,        Description = "Edit roles",             Group = "Roles",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.RolesDelete,      Description = "Delete roles",           Group = "Roles",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.RolesAssign,      Description = "Assign roles",           Group = "Roles",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.PermissionsView,  Description = "View permissions",       Group = "Permissions", CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.PermissionsManage,Description = "Manage permissions",     Group = "Permissions", CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.SettingsView,     Description = "View settings",          Group = "Settings",    CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.SettingsUpdate,   Description = "Update settings",        Group = "Settings",    CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.FilesUpload,      Description = "Upload files",           Group = "Files",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.FilesDelete,      Description = "Delete files",           Group = "Files",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.AuditView,        Description = "View audit logs",        Group = "Audit",       CreatedAt = now },
            new Permission { Id = Guid.NewGuid(), Name = Permissions.AdminAccess,      Description = "Access admin panel",     Group = "Admin",       CreatedAt = now }
        ];
    }
}