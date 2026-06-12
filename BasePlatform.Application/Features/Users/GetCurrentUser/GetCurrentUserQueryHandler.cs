using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Application.Features.Users.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler
    : IQueryHandler<GetCurrentUserQuery, Result<UserProfileResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUser _currentUser;

    public GetCurrentUserQueryHandler(
        UserManager<AppUser> userManager,
        ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<Result<UserProfileResponse>> HandleAsync(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Result<UserProfileResponse>.Failure(
                Error.Unauthorized("Users.Unauthenticated", "User is not authenticated."));

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user is null || !user.IsActive)
            return Result<UserProfileResponse>.Failure(
                Error.NotFound("Users.NotFound", "User not found."));

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var permissions = claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        return Result<UserProfileResponse>.Success(new UserProfileResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.DisplayName,
            user.Email!,
            user.IsActive,
            roles.ToList(),
            permissions,
            user.CreatedAt));
    }
}