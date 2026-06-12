namespace BasePlatform.Application.Features.AdminAuth.AdminLogin;

public sealed record AdminLoginResponse(
    Guid UserId,
    string Email,
    string DisplayName);