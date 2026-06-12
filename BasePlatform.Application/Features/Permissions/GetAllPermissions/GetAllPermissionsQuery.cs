using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Permissions.GetAllPermissions;

public sealed record GetAllPermissionsQuery : IQuery<Result<List<PermissionDto>>>;