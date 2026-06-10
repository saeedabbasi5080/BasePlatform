namespace BasePlatform.Application.Common.Abstractions
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(Guid userId, string email, IReadOnlyList<string> roles, IReadOnlyList<string> permissions);
        Guid? GetUserIdFromExpiredToken(string token);
    }
}
