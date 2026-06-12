using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Audit.GetAuditLogs;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Shared;
using Dapper;

namespace BasePlatform.Infrastructure.Queries.Audit;

public sealed class GetAuditLogsQueryHandler
    : IQueryHandler<GetAuditLogsQuery, Result<PaginatedResult<AuditLogDto>>>
{
    private readonly IDapperQueryConnection _db;

    public GetAuditLogsQueryHandler(IDapperQueryConnection db)
    {
        _db = db;
    }

    public async Task<Result<PaginatedResult<AuditLogDto>>> HandleAsync(
        GetAuditLogsQuery query,
        CancellationToken cancellationToken = default)
    {
        var offset = (query.Page - 1) * query.PageSize;

        var whereClause = BuildWhereClause(query);

        var countSql = $"""
            SELECT COUNT(*)
            FROM AuditLogs
            {whereClause}
            """;

        var dataSql = $"""
            SELECT
                Id,
                ActorId,
                ActorEmail,
                Action,
                TargetEntityType,
                TargetEntityId,
                Details,
                IpAddress,
                CreatedAt
            FROM AuditLogs
            {whereClause}
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", query.PageSize);

        if (!string.IsNullOrWhiteSpace(query.ActorEmail))
            parameters.Add("ActorEmail", $"%{query.ActorEmail}%");

        if (!string.IsNullOrWhiteSpace(query.Action))
            parameters.Add("Action", query.Action);

        if (!string.IsNullOrWhiteSpace(query.TargetEntityType))
            parameters.Add("TargetEntityType", query.TargetEntityType);

        if (query.From.HasValue)
            parameters.Add("From", query.From.Value);

        if (query.To.HasValue)
            parameters.Add("To", query.To.Value);

        using var connection = _db.CreateConnection();

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await connection.QueryAsync<AuditLogDto>(dataSql, parameters);

        var result = new PaginatedResult<AuditLogDto>(
            items.ToList(),
            totalCount,
            query.Page,
            query.PageSize);

        return Result<PaginatedResult<AuditLogDto>>.Success(result);
    }

    private static string BuildWhereClause(GetAuditLogsQuery query)
    {
        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.ActorEmail))
            conditions.Add("ActorEmail LIKE @ActorEmail");

        if (!string.IsNullOrWhiteSpace(query.Action))
            conditions.Add("Action = @Action");

        if (!string.IsNullOrWhiteSpace(query.TargetEntityType))
            conditions.Add("TargetEntityType = @TargetEntityType");

        if (query.From.HasValue)
            conditions.Add("CreatedAt >= @From");

        if (query.To.HasValue)
            conditions.Add("CreatedAt <= @To");

        return conditions.Count > 0
            ? "WHERE " + string.Join(" AND ", conditions)
            : string.Empty;
    }
}