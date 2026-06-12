namespace BasePlatform.Application.Features.Files.UploadFile;

public sealed record UploadFileResponse(
    Guid Id,
    string FileName,
    string OriginalFileName,
    string ContentType,
    long FileSizeBytes,
    string StoragePath,
    string StorageProvider,
    DateTimeOffset CreatedAt);