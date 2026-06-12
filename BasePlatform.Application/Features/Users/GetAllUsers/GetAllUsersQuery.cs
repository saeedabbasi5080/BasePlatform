using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Users.GetAllUsers;

public sealed record GetAllUsersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null) : IQuery<Result<PaginatedResult<UserSummaryDto>>>;