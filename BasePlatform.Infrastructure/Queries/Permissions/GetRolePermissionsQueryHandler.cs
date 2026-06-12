using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Permissions.GetAllPermissions;
using BasePlatform.Application.Features.Permissions.GetRolePermissions;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Shared;
using Dapper;

namespace BasePlatform.Infrastructure.Queries.Permissions;

public sealed class GetRolePermissionsQueryHandler
    : IQueryHandler<GetRolePermissionsQuery, Result<List<PermissionDto>>>
{
    private readonly IDapperQueryConnection _db;

    public GetRolePermissionsQueryHandler(IDapperQueryConnection db)
    {
        _db = db;
    }

    public async Task<Result<List<PermissionDto>>> HandleAsync(
        GetRolePermissionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var sql = """
            SELECT p.Id, p.Name, p.Description, p.[Group]
            FROM Permissions p
            INNER JOIN RolePermissions rp ON rp.PermissionId = p.Id
            WHERE rp.RoleId = @RoleId
            ORDER BY p.[Group] ASC, p.Name ASC;
            """;

        using var connection = _db.CreateConnection();
        var permissions = (await connection.QueryAsync<PermissionDto>(
            sql, new { RoleId = query.RoleId })).ToList();

        return Result<List<PermissionDto>>.Success(permissions);
    }
}