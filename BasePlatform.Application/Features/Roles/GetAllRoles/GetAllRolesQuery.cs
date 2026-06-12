using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Roles.GetAllRoles;

public sealed record GetAllRolesQuery : IQuery<Result<List<RoleSummaryDto>>>;