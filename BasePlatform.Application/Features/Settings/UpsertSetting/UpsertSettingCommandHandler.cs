using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Settings.UpsertSetting;

public sealed class UpsertSettingCommandHandler
    : ICommandHandler<UpsertSettingCommand, Result>
{
    private readonly ISettingRepository _settingRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpsertSettingCommandHandler(
        ISettingRepository settingRepository,
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _settingRepository = settingRepository;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> HandleAsync(
        UpsertSettingCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Key))
            return Result.Failure(
                Error.Validation("Settings.InvalidKey", "Key cannot be empty."));

        var existing = await _settingRepository.GetByKeyAsync(
            command.Key, cancellationToken);

        if (existing is null)
        {
            var newSetting = new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = command.Key,
                Value = command.Value,
                Description = command.Description,
                IsPublic = command.IsPublic,
                UpdatedAt = _dateTimeProvider.UtcNow,
                UpdatedByUserId = _currentUser.UserId
            };

            await _settingRepository.UpsertAsync(newSetting, cancellationToken);
        }
        else
        {
            existing.Value = command.Value;
            existing.Description = command.Description;
            existing.IsPublic = command.IsPublic;
            existing.UpdatedAt = _dateTimeProvider.UtcNow;
            existing.UpdatedByUserId = _currentUser.UserId;

            await _settingRepository.UpsertAsync(existing, cancellationToken);
        }

        return Result.Success();
    }
}