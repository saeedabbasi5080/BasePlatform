using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Permissions.GetAllPermissions;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Shared;
using Dapper;

namespace BasePlatform.Infrastructure.Queries.Permissions;

public sealed class GetAllPermissionsQueryHandler
    : IQueryHandler<GetAllPermissionsQuery, Result<List<PermissionDto>>>
{
    private readonly IDapperQueryConnection _db;

    public GetAllPermissionsQueryHandler(IDapperQueryConnection db)
    {
        _db = db;
    }

    public async Task<Result<List<PermissionDto>>> HandleAsync(
        GetAllPermissionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var sql = """
            SELECT Id, Name, Description, [Group]
            FROM Permissions
            ORDER BY [Group] ASC, Name ASC;
            """;

        using var connection = _db.CreateConnection();
        var permissions = (await connection.QueryAsync<PermissionDto>(sql)).ToList();

        return Result<List<PermissionDto>>.Success(permissions);
    }
}