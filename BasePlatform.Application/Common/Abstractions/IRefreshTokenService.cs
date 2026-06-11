using BasePlatform.Shared;

namespace BasePlatform.Application.Common.Abstractions;

public interface IRefreshTokenService
{
    string GenerateToken();
    string HashToken(string token);

    Task<bool> ValidateAsync(
        Guid userId,
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task RevokeAsync(
        Guid userId,
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task RevokeAllAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}