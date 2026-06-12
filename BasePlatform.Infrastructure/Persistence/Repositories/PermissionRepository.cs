using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasePlatform.Infrastructure.Persistence.Repositories;

public sealed class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _context;

    public PermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Permission>> GetByIdsAsync(
        List<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task ReplaceRolePermissionsAsync(
        Guid roleId,
        List<Permission> permissions,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);

        _context.RolePermissions.RemoveRange(existing);

        var newEntries = permissions.Select(p => new RolePermission
        {
            RoleId = roleId,
            PermissionId = p.Id
        });

        await _context.RolePermissions.AddRangeAsync(newEntries, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}