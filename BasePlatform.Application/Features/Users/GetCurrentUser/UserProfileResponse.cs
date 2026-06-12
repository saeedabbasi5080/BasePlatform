namespace BasePlatform.Application.Features.Users.GetCurrentUser;

public sealed record UserProfileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    bool IsActive,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    DateTimeOffset CreatedAt);