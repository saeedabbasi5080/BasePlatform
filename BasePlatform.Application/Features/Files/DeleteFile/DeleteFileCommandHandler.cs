using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Files.DeleteFile;

public sealed class DeleteFileCommandHandler
    : ICommandHandler<DeleteFileCommand, Result>
{
    private readonly IStorageService _storageService;
    private readonly IStoredFileRepository _storedFileRepository;

    public DeleteFileCommandHandler(
        IStorageService storageService,
        IStoredFileRepository storedFileRepository)
    {
        _storageService = storageService;
        _storedFileRepository = storedFileRepository;
    }

    public async Task<Result> HandleAsync(
        DeleteFileCommand command,
        CancellationToken cancellationToken = default)
    {
        var file = await _storedFileRepository.GetByIdAsync(
            command.FileId, cancellationToken);

        if (file is null)
            return Result.Failure(
                Error.NotFound("Files.NotFound", "File not found."));

        try
        {
            await _storageService.DeleteAsync(file.StoragePath, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Failure("Files.DeleteFailed", $"File deletion failed: {ex.Message}"));
        }

        await _storedFileRepository.DeleteAsync(file, cancellationToken);

        return Result.Success();
    }
}