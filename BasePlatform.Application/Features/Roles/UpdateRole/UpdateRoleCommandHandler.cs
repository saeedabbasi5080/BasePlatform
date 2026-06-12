using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Application.Features.Roles.UpdateRole;

public sealed class UpdateRoleCommandHandler
    : ICommandHandler<UpdateRoleCommand, Result>
{
    private readonly RoleManager<AppRole> _roleManager;

    public UpdateRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result> HandleAsync(
        UpdateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(command.RoleId.ToString());
        if (role is null)
            return Result.Failure(
                Error.NotFound("Roles.NotFound", "Role not found."));

        var builtInRoles = new[] { "SuperAdmin", "Admin", "User" };
        if (builtInRoles.Contains(role.Name, StringComparer.OrdinalIgnoreCase))
            return Result.Failure(
                Error.Forbidden("Roles.CannotModifyBuiltIn", "Built-in roles cannot be modified."));

        role.Name = command.Name;
        role.Description = command.Description;

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(Error.Validation("Roles.UpdateFailed", errorMessage));
        }

        return Result.Success();
    }
}