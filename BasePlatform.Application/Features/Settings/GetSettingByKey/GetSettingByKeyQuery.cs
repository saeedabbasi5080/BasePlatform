using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Settings.GetSettings;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Settings.GetSettingByKey;

public sealed record GetSettingByKeyQuery(string Key)
    : IQuery<Result<AppSettingDto>>;