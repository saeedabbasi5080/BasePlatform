using BasePlatform.Admin.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BasePlatform.Admin.DependencyInjection;

public static class AdminServiceExtensions
{
    public static IServiceCollection AddAdminServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        services
            .AddAuthentication(AdminCookieDefaults.AuthenticationScheme)
            .AddCookie(AdminCookieDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "BasePlatform.Admin";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = ctx =>
                    {
                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}