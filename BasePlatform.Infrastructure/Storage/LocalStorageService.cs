using BasePlatform.Application.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace BasePlatform.Infrastructure.Storage;

public sealed class LocalStorageService : IStorageService
{
    private readonly string _basePath;

    public LocalStorageService(IConfiguration configuration)
    {
        _basePath = configuration["Storage:Local:BasePath"] ?? "./uploads";

        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, fileName);

        await using var fs = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await fileStream.CopyToAsync(fs, cancellationToken);

        return filePath;
    }

    public Task<Stream> DownloadAsync(
        string storagePath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(storagePath))
            throw new FileNotFoundException("File not found.", storagePath);

        Stream stream = new FileStream(
            storagePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        return Task.FromResult(stream);
    }

    public Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken = default)
    {
        if (File.Exists(storagePath))
            File.Delete(storagePath);

        return Task.CompletedTask;
    }
}