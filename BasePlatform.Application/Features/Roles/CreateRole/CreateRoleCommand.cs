using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Roles.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    string Description) : ICommand<Result<Guid>>;