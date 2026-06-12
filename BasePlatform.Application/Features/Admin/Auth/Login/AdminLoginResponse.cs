namespace BasePlatform.Application.Features.Admin.Auth.Login;

public sealed record AdminLoginResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);