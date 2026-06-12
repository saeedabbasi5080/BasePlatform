using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Roles.GetRoleById;

public sealed record GetRoleByIdQuery(Guid RoleId) : IQuery<Result<RoleDetailResponse>>;