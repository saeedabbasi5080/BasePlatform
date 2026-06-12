using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Users.GetAllUsers;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Shared;
using Dapper;

namespace BasePlatform.Infrastructure.Queries.Users;

public sealed class GetAllUsersQueryHandler
    : IQueryHandler<GetAllUsersQuery, Result<PaginatedResult<UserSummaryDto>>>
{
    private readonly IDapperQueryConnection _db;

    public GetAllUsersQueryHandler(IDapperQueryConnection db)
    {
        _db = db;
    }

    public async Task<Result<PaginatedResult<UserSummaryDto>>> HandleAsync(
        GetAllUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        var offset = (query.Page - 1) * query.PageSize;

        var sql = """
            SELECT
                u.Id,
                u.FirstName,
                u.LastName,
                u.DisplayName,
                u.Email,
                u.IsActive,
                u.CreatedAt
            FROM AspNetUsers u
            WHERE (@Search IS NULL
                OR u.Email LIKE '%' + @Search + '%'
                OR u.FirstName LIKE '%' + @Search + '%'
                OR u.LastName LIKE '%' + @Search + '%')
            ORDER BY u.CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*)
            FROM AspNetUsers u
            WHERE (@Search IS NULL
                OR u.Email LIKE '%' + @Search + '%'
                OR u.FirstName LIKE '%' + @Search + '%'
                OR u.LastName LIKE '%' + @Search + '%');
            """;

        using var connection = _db.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(sql, new
        {
            Search = query.Search,
            Offset = offset,
            PageSize = query.PageSize
        });

        var users = (await multi.ReadAsync<UserSummaryDto>()).ToList();
        var totalCount = await multi.ReadFirstAsync<int>();

        return Result<PaginatedResult<UserSummaryDto>>.Success(
            new PaginatedResult<UserSummaryDto>(users, totalCount, query.Page, query.PageSize));
    }
}