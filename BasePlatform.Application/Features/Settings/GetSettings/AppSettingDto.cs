namespace BasePlatform.Application.Features.Settings.GetSettings;

public sealed record AppSettingDto(
    Guid Id,
    string Key,
    string Value,
    string Description,
    bool IsPublic,
    DateTimeOffset UpdatedAt);