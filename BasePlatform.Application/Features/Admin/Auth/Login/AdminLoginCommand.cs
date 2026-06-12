using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.AdminAuth.AdminLogin;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Admin.Auth.Login;

public sealed record AdminLoginCommand(
    string Email,
    string Password,
    bool RememberMe) : ICommand<Result<AdminLoginResponse>>;