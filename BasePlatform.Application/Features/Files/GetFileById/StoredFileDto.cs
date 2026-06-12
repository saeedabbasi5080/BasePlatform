namespace BasePlatform.Application.Features.Files.GetFileById;

public sealed record StoredFileDto(
    Guid Id,
    string FileName,
    string OriginalFileName,
    string ContentType,
    long FileSizeBytes,
    string StoragePath,
    string StorageProvider,
    Guid UploadedByUserId,
    DateTimeOffset CreatedAt);