using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand<Result>;