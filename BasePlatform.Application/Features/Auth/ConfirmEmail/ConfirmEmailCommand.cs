using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Auth.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string UserId,
    string Token) : ICommand<Result>;