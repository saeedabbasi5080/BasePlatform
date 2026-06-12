using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Roles.GetAllRoles;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Shared;
using Dapper;

namespace BasePlatform.Infrastructure.Queries.Roles;

public sealed class GetAllRolesQueryHandler
    : IQueryHandler<GetAllRolesQuery, Result<List<RoleSummaryDto>>>
{
    private readonly IDapperQueryConnection _db;

    public GetAllRolesQueryHandler(IDapperQueryConnection db)
    {
        _db = db;
    }

    public async Task<Result<List<RoleSummaryDto>>> HandleAsync(
        GetAllRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        var sql = """
            SELECT
                r.Id,
                r.Name,
                r.Description
            FROM AspNetRoles r
            ORDER BY r.Name ASC;
            """;

        using var connection = _db.CreateConnection();
        var roles = (await connection.QueryAsync<RoleSummaryDto>(sql)).ToList();

        return Result<List<RoleSummaryDto>>.Success(roles);
    }
}