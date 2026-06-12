using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Users.AssignRole;

public sealed record AssignRoleCommand(
    Guid UserId,
    string RoleName) : ICommand<Result>;