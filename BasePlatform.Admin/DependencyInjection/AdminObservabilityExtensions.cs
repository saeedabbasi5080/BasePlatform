using BasePlatform.Infrastructure;
using BasePlatform.Infrastructure.Persistence;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BasePlatform.Admin.DependencyInjection;

public static class AdminObservabilityExtensions
{
    public static IServiceCollection AddAdminObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Health Checks
        services
            .AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("database");

        // OpenTelemetry
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(
                    serviceName: "BasePlatform.Admin",
                    serviceVersion: "1.0.0"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter();
            });

        return services;
    }
}