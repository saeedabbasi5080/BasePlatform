using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Files.UploadFile;

public sealed class UploadFileCommandHandler
    : ICommandHandler<UploadFileCommand, Result<UploadFileResponse>>
{
    private readonly IStorageService _storageService;
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UploadFileCommandHandler(
        IStorageService storageService,
        IStoredFileRepository storedFileRepository,
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _storageService = storageService;
        _storedFileRepository = storedFileRepository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<UploadFileResponse>> HandleAsync(
        UploadFileCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.FileStream is null || command.FileStream.Length == 0)
            return Result<UploadFileResponse>.Failure(
                Error.Validation("Files.EmptyFile", "File stream is empty."));

        if (string.IsNullOrWhiteSpace(command.OriginalFileName))
            return Result<UploadFileResponse>.Failure(
                Error.Validation("Files.InvalidFileName", "File name is required."));

        var extension = Path.GetExtension(command.OriginalFileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        string storagePath;
        try
        {
            storagePath = await _storageService.UploadAsync(
                command.FileStream,
                uniqueFileName,
                command.ContentType,
                cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<UploadFileResponse>.Failure(
                Error.Failure("Files.UploadFailed", $"File upload failed: {ex.Message}"));
        }

        var storedFile = new StoredFile
        {
            Id = Guid.NewGuid(),
            FileName = uniqueFileName,
            OriginalFileName = command.OriginalFileName,
            ContentType = command.ContentType,
            FileSizeBytes = command.FileSizeBytes,
            StoragePath = storagePath,
            StorageProvider = "local",
            UploadedByUserId = _currentUser.UserId ?? Guid.Empty,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _storedFileRepository.AddAsync(storedFile, cancellationToken);

        return Result<UploadFileResponse>.Success(new UploadFileResponse(
            storedFile.Id,
            storedFile.FileName,
            storedFile.OriginalFileName,
            storedFile.ContentType,
            storedFile.FileSizeBytes,
            storedFile.StoragePath,
            storedFile.StorageProvider,
            storedFile.CreatedAt));
    }
}