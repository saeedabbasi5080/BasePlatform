using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Permissions.GetAllPermissions;
using BasePlatform.Application.Features.Roles.GetRoleById;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Shared;
using Dapper;

namespace BasePlatform.Infrastructure.Queries.Roles;

public sealed class GetRoleByIdQueryHandler
    : IQueryHandler<GetRoleByIdQuery, Result<RoleDetailResponse>>
{
    private readonly IDapperQueryConnection _db;

    public GetRoleByIdQueryHandler(IDapperQueryConnection db)
    {
        _db = db;
    }

    public async Task<Result<RoleDetailResponse>> HandleAsync(
        GetRoleByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var roleSql = """
            SELECT Id, Name, Description
            FROM AspNetRoles
            WHERE Id = @RoleId;
            """;

        var permissionsSql = """
            SELECT p.Id, p.Name, p.Description, p.Group
            FROM Permissions p
            INNER JOIN RolePermissions rp ON rp.PermissionId = p.Id
            WHERE rp.RoleId = @RoleId;
            """;

        using var connection = _db.CreateConnection();

        var role = await connection.QueryFirstOrDefaultAsync<(Guid Id, string Name, string Description)>(
            roleSql, new { RoleId = query.RoleId });

        if (role == default)
            return Result<RoleDetailResponse>.Failure(
                Error.NotFound("Roles.NotFound", "Role not found."));

        var permissions = (await connection.QueryAsync<PermissionDto>(
            permissionsSql, new { RoleId = query.RoleId })).ToList();

        return Result<RoleDetailResponse>.Success(
            new RoleDetailResponse(role.Id, role.Name, role.Description, permissions));
    }
}