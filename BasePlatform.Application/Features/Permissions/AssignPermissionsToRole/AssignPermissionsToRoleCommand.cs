using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Permissions.AssignPermissionsToRole;

public sealed record AssignPermissionsToRoleCommand(
    Guid RoleId,
    List<Guid> PermissionIds) : ICommand<Result>;