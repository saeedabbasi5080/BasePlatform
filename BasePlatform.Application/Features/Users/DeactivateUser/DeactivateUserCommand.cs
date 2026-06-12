using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Users.DeactivateUser;

public sealed record DeactivateUserCommand(Guid UserId) : ICommand<Result>;