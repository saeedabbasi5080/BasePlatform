using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Files.GetFileById;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Shared;
using Dapper;

namespace BasePlatform.Infrastructure.Queries.Files;

public sealed class GetFileByIdQueryHandler
    : IQueryHandler<GetFileByIdQuery, Result<StoredFileDto>>
{
    private readonly IDapperQueryConnection _db;

    public GetFileByIdQueryHandler(IDapperQueryConnection db)
    {
        _db = db;
    }

    public async Task<Result<StoredFileDto>> HandleAsync(
        GetFileByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var sql = """
            SELECT
                Id,
                FileName,
                OriginalFileName,
                ContentType,
                FileSizeBytes,
                StoragePath,
                StorageProvider,
                UploadedByUserId,
                CreatedAt
            FROM StoredFiles
            WHERE Id = @FileId;
            """;

        using var connection = _db.CreateConnection();
        var file = await connection.QueryFirstOrDefaultAsync<StoredFileDto>(
            sql, new { FileId = query.FileId });

        if (file is null)
            return Result<StoredFileDto>.Failure(
                Error.NotFound("Files.NotFound", "File not found."));

        return Result<StoredFileDto>.Success(file);
    }
}