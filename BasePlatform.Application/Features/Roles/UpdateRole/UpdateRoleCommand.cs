using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Roles.UpdateRole;

public sealed record UpdateRoleCommand(
    Guid RoleId,
    string Name,
    string Description) : ICommand<Result>;