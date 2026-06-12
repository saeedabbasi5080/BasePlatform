using BasePlatform.Domain.Entities;

namespace BasePlatform.Application.Common.Abstractions;

public interface ISettingRepository
{
    Task<AppSetting?> GetByKeyAsync(
        string key,
        CancellationToken cancellationToken = default);

    Task UpsertAsync(
        AppSetting setting,
        CancellationToken cancellationToken = default);
}