using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Users.GetCurrentUser;

public sealed record GetCurrentUserQuery : IQuery<Result<UserProfileResponse>>;