using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Application.Features.Users.ChangePassword;

public sealed class ChangePasswordCommandHandler
    : ICommandHandler<ChangePasswordCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUser _currentUser;

    public ChangePasswordCommandHandler(
        UserManager<AppUser> userManager,
        ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<Result> HandleAsync(
        ChangePasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Result.Failure(
                Error.Unauthorized("Users.Unauthenticated", "User is not authenticated."));

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user is null || !user.IsActive)
            return Result.Failure(
                Error.NotFound("Users.NotFound", "User not found."));

        var result = await _userManager.ChangePasswordAsync(
            user, command.CurrentPassword, command.NewPassword);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(Error.Validation("Users.ChangePasswordFailed", errorMessage));
        }

        return Result.Success();
    }
}