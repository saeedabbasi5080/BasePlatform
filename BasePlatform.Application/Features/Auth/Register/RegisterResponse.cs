namespace BasePlatform.Application.Features.Auth.Register;

public sealed record RegisterResponse(
    Guid UserId,
    string Email,
    string Message);