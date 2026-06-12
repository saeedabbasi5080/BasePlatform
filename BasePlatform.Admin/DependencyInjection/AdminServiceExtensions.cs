using BasePlatform.Admin.Configuration;
using BasePlatform.Domain.Constants;
using BasePlatform.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace BasePlatform.Admin.DependencyInjection;

public static class AdminServiceExtensions
{
    public static IServiceCollection AddAdminServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BasePlatform Admin API",
                Version = "v1",
                Description = "BasePlatform Admin Panel API (Cookie Auth)"
            });
        });

        // Cookie Authentication
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

        // Authorization Policies
        services.AddAuthorization(options =>
        {
            foreach (var permission in Permissions.All)
            {
                options.AddPolicy(permission, policy =>
                    policy.Requirements.Add(new PermissionRequirement(permission)));
            }
        });

        return services;
    }
}