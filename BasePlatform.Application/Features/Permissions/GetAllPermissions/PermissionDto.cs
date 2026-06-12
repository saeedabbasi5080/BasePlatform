namespace BasePlatform.Application.Features.Permissions.GetAllPermissions;

public sealed record PermissionDto(
    Guid Id,
    string Name,
    string Description,
    string Group);