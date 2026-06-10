namespace BasePlatform.Application.Common.Abstractions
{
    public interface IStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);
        string GetUrl(string storagePath, CancellationToken cancellationToken = default);
    }
}
