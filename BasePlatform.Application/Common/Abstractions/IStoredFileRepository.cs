using BasePlatform.Domain.Entities;

namespace BasePlatform.Application.Common.Abstractions;

public interface IStoredFileRepository
{
    Task<StoredFile?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        StoredFile file,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        StoredFile file,
        CancellationToken cancellationToken = default);
}