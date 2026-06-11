using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Auth.Logout;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand, Result>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRefreshTokenService _refreshTokenService;

    public LogoutCommandHandler(
        ICurrentUser currentUser,
        IRefreshTokenService refreshTokenService)
    {
        _currentUser = currentUser;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result> HandleAsync(
        LogoutCommand command,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Result.Failure(Error.Unauthorized("Auth.Unauthenticated", "User is not authenticated."));

        var tokenHash = _refreshTokenService.HashToken(command.RefreshToken);

        await _refreshTokenService.RevokeAsync(
            _currentUser.UserId.Value,
            tokenHash,
            cancellationToken);

        return Result.Success();
    }
}