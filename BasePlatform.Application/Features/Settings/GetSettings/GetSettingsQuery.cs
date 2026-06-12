using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Settings.GetSettings;

public sealed record GetSettingsQuery(bool PublicOnly = false)
    : IQuery<Result<List<AppSettingDto>>>;