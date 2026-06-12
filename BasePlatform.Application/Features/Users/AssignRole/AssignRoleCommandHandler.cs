using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Application.Features.Users.AssignRole;

public sealed class AssignRoleCommandHandler
    : ICommandHandler<AssignRoleCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public AssignRoleCommandHandler(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result> HandleAsync(
        AssignRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null || !user.IsActive)
            return Result.Failure(
                Error.NotFound("Users.NotFound", "User not found."));

        var roleExists = await _roleManager.RoleExistsAsync(command.RoleName);
        if (!roleExists)
            return Result.Failure(
                Error.NotFound("Roles.NotFound", $"Role '{command.RoleName}' does not exist."));

        var alreadyInRole = await _userManager.IsInRoleAsync(user, command.RoleName);
        if (alreadyInRole)
            return Result.Failure(
                Error.Conflict("Users.AlreadyInRole", $"User already has the '{command.RoleName}' role."));

        var result = await _userManager.AddToRoleAsync(user, command.RoleName);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(Error.Validation("Users.AssignRoleFailed", errorMessage));
        }

        return Result.Success();
    }
}