using BasePlatform.Application.Features.Permissions.GetAllPermissions;

namespace BasePlatform.Application.Features.Roles.GetRoleById;

public sealed record RoleDetailResponse(
    Guid Id,
    string Name,
    string Description,
    List<PermissionDto> Permissions);