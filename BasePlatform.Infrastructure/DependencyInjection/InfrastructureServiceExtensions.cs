using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Auth.ConfirmEmail;
using BasePlatform.Application.Features.Auth.ForgotPassword;
using BasePlatform.Application.Features.Auth.Login;
using BasePlatform.Application.Features.Auth.Logout;
using BasePlatform.Application.Features.Auth.RefreshToken;
using BasePlatform.Application.Features.Auth.Register;
using BasePlatform.Application.Features.Auth.ResetPassword;
using BasePlatform.Infrastructure.Authentication;
using BasePlatform.Infrastructure.Dispatcher;
using BasePlatform.Infrastructure.Identity;
using BasePlatform.Infrastructure.Persistence;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Infrastructure.Services;
using BasePlatform.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BasePlatform.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IDapperQueryConnection, DapperQueryConnection>();

        services.AddIdentityServices();
        services.AddScoped<IdentitySeeder>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IDispatcher, BasePlatform.Infrastructure.Dispatcher.Dispatcher>();

        services.AddScoped<ICommandHandler<RegisterCommand, Result<RegisterResponse>>, RegisterCommandHandler>();
        services.AddScoped<ICommandHandler<LoginCommand, Result<LoginResponse>>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<LogoutCommand, Result>, LogoutCommandHandler>();
        services.AddScoped<ICommandHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>, RefreshTokenCommandHandler>();
        services.AddScoped<ICommandHandler<ConfirmEmailCommand, Result>, ConfirmEmailCommandHandler>();
        services.AddScoped<ICommandHandler<ForgotPasswordCommand, Result>, ForgotPasswordCommandHandler>();
        services.AddScoped<ICommandHandler<ResetPasswordCommand, Result>, ResetPasswordCommandHandler>();

        return services;
    }
}