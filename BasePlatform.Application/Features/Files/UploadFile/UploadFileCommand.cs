using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Files.UploadFile;

public sealed record UploadFileCommand(
    Stream FileStream,
    string OriginalFileName,
    string ContentType,
    long FileSizeBytes) : ICommand<Result<UploadFileResponse>>;