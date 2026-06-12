using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Files.GetFileById;

public sealed record GetFileByIdQuery(Guid FileId)
    : IQuery<Result<StoredFileDto>>;