using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Users.GetCurrentUser;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<Result<UserProfileResponse>>;