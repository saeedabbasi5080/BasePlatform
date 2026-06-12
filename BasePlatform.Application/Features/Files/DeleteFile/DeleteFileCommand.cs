using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Files.DeleteFile;

public sealed record DeleteFileCommand(Guid FileId) : ICommand<Result>;