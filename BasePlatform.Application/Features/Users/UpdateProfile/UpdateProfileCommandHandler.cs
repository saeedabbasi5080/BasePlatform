using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Application.Features.Users.UpdateProfile;

public sealed class UpdateProfileCommandHandler
    : ICommandHandler<UpdateProfileCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateProfileCommandHandler(
        UserManager<AppUser> userManager,
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> HandleAsync(
        UpdateProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Result.Failure(
                Error.Unauthorized("Users.Unauthenticated", "User is not authenticated."));

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user is null || !user.IsActive)
            return Result.Failure(
                Error.NotFound("Users.NotFound", "User not found."));

        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.DisplayName = command.DisplayName;
        user.UpdatedAt = _dateTimeProvider.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(Error.Validation("Users.UpdateFailed", errorMessage));
        }

        return Result.Success();
    }
}