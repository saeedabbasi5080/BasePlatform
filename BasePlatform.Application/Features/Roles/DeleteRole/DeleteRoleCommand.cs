using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Roles.DeleteRole;

public sealed record DeleteRoleCommand(Guid RoleId) : ICommand<Result>;