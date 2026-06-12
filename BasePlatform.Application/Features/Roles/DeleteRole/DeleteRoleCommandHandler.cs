using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Application.Features.Roles.DeleteRole;

public sealed class DeleteRoleCommandHandler
    : ICommandHandler<DeleteRoleCommand, Result>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public DeleteRoleCommandHandler(
        RoleManager<AppRole> roleManager,
        UserManager<AppUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<Result> HandleAsync(
        DeleteRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(command.RoleId.ToString());
        if (role is null)
            return Result.Failure(
                Error.NotFound("Roles.NotFound", "Role not found."));

        var builtInRoles = new[] { "SuperAdmin", "Admin", "User" };
        if (builtInRoles.Contains(role.Name, StringComparer.OrdinalIgnoreCase))
            return Result.Failure(
                Error.Forbidden("Roles.CannotDeleteBuiltIn", "Built-in roles cannot be deleted."));

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Count > 0)
            return Result.Failure(
                Error.Conflict("Roles.HasUsers", "Cannot delete a role that has assigned users."));

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(Error.Validation("Roles.DeleteFailed", errorMessage));
        }

        return Result.Success();
    }
}