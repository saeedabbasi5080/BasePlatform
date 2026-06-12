namespace BasePlatform.Application.Features.Roles.GetAllRoles;

public sealed record RoleSummaryDto(
    Guid Id,
    string Name,
    string Description);