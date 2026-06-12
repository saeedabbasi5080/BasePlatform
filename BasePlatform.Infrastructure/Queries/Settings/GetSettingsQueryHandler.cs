using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Settings.GetSettings;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Shared;
using Dapper;

namespace BasePlatform.Infrastructure.Queries.Settings;

public sealed class GetSettingsQueryHandler
    : IQueryHandler<GetSettingsQuery, Result<List<AppSettingDto>>>
{
    private readonly IDapperQueryConnection _db;

    public GetSettingsQueryHandler(IDapperQueryConnection db)
    {
        _db = db;
    }

    public async Task<Result<List<AppSettingDto>>> HandleAsync(
        GetSettingsQuery query,
        CancellationToken cancellationToken = default)
    {
        var sql = query.PublicOnly
            ? """
              SELECT Id, [Key], Value, Description, IsPublic, UpdatedAt
              FROM AppSettings
              WHERE IsPublic = 1
              ORDER BY [Key] ASC;
              """
            : """
              SELECT Id, [Key], Value, Description, IsPublic, UpdatedAt
              FROM AppSettings
              ORDER BY [Key] ASC;
              """;

        using var connection = _db.CreateConnection();
        var settings = (await connection.QueryAsync<AppSettingDto>(sql)).ToList();

        return Result<List<AppSettingDto>>.Success(settings);
    }
}