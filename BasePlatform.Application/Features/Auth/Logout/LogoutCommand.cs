using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Auth.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand<Result>;