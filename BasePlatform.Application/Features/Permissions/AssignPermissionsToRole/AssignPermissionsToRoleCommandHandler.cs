using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BasePlatform.Application.Features.Permissions.AssignPermissionsToRole;

public sealed class AssignPermissionsToRoleCommandHandler
    : ICommandHandler<AssignPermissionsToRoleCommand, Result>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IPermissionRepository _permissionRepository;

    public AssignPermissionsToRoleCommandHandler(
        RoleManager<AppRole> roleManager,
        IPermissionRepository permissionRepository)
    {
        _roleManager = roleManager;
        _permissionRepository = permissionRepository;
    }

    public async Task<Result> HandleAsync(
        AssignPermissionsToRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(command.RoleId.ToString());
        if (role is null)
            return Result.Failure(
                Error.NotFound("Roles.NotFound", "Role not found."));

        var permissions = await _permissionRepository
            .GetByIdsAsync(command.PermissionIds, cancellationToken);

        var invalidIds = command.PermissionIds
            .Except(permissions.Select(p => p.Id))
            .ToList();

        if (invalidIds.Count > 0)
            return Result.Failure(
                Error.Validation("Permissions.InvalidIds",
                    $"Permission IDs not found: {string.Join(", ", invalidIds)}"));

        await _permissionRepository
            .ReplaceRolePermissionsAsync(command.RoleId, permissions, cancellationToken);

        return Result.Success();
    }
}