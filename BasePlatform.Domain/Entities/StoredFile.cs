namespace BasePlatform.Domain.Entities;

public class StoredFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string StorageProvider { get; set; } = string.Empty;
    public Guid UploadedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}