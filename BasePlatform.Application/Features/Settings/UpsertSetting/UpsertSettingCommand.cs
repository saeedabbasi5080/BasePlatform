using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Settings.UpsertSetting;

public sealed record UpsertSettingCommand(
    string Key,
    string Value,
    string Description,
    bool IsPublic) : ICommand<Result>;