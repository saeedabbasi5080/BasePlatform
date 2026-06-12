using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Settings.GetSettingByKey;
using BasePlatform.Application.Features.Settings.GetSettings;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Shared;
using Dapper;

namespace BasePlatform.Infrastructure.Queries.Settings;

public sealed class GetSettingByKeyQueryHandler
    : IQueryHandler<GetSettingByKeyQuery, Result<AppSettingDto>>
{
    private readonly IDapperQueryConnection _db;

    public GetSettingByKeyQueryHandler(IDapperQueryConnection db)
    {
        _db = db;
    }

    public async Task<Result<AppSettingDto>> HandleAsync(
        GetSettingByKeyQuery query,
        CancellationToken cancellationToken = default)
    {
        var sql = """
            SELECT Id, [Key], Value, Description, IsPublic, UpdatedAt
            FROM AppSettings
            WHERE [Key] = @Key;
            """;

        using var connection = _db.CreateConnection();
        var setting = await connection.QueryFirstOrDefaultAsync<AppSettingDto>(
            sql, new { Key = query.Key });

        if (setting is null)
            return Result<AppSettingDto>.Failure(
                Error.NotFound("Settings.NotFound", $"Setting '{query.Key}' not found."));

        return Result<AppSettingDto>.Success(setting);
    }
}