using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Permissions.GetAllPermissions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Permissions.GetRolePermissions;

public sealed record GetRolePermissionsQuery(Guid RoleId)
    : IQuery<Result<List<PermissionDto>>>;