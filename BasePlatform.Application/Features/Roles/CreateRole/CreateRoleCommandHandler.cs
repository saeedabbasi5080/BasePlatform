using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Application.Features.Roles.CreateRole;

public sealed class CreateRoleCommandHandler
    : ICommandHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly RoleManager<AppRole> _roleManager;

    public CreateRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<Guid>> HandleAsync(
        CreateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var exists = await _roleManager.RoleExistsAsync(command.Name);
        if (exists)
            return Result<Guid>.Failure(
                Error.Conflict("Roles.AlreadyExists", $"Role '{command.Name}' already exists."));

        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result<Guid>.Failure(Error.Validation("Roles.CreateFailed", errorMessage));
        }

        return Result<Guid>.Success(role.Id);
    }
}