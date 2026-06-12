using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Application.Features.Users.DeactivateUser;

public sealed class DeactivateUserCommandHandler
    : ICommandHandler<DeactivateUserCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeactivateUserCommandHandler(
        UserManager<AppUser> userManager,
        IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> HandleAsync(
        DeactivateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return Result.Failure(
                Error.NotFound("Users.NotFound", "User not found."));

        if (!user.IsActive)
            return Result.Failure(
                Error.Conflict("Users.AlreadyInactive", "User is already deactivated."));

        user.IsActive = false;
        user.UpdatedAt = _dateTimeProvider.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(Error.Validation("Users.DeactivateFailed", errorMessage));
        }

        return Result.Success();
    }
}