namespace BasePlatform.Application.Features.Users.GetAllUsers;

public sealed record UserSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    bool IsActive,
    DateTimeOffset CreatedAt);