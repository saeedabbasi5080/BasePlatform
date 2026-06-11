using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<Result<LoginResponse>>;