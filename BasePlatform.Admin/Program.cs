using BasePlatform.Admin.DependencyInjection;
using BasePlatform.Admin.Middleware;
using BasePlatform.Infrastructure.DependencyInjection;
using BasePlatform.Infrastructure.Identity;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, config) =>
        config.ReadFrom.Configuration(context.Configuration)
              .ReadFrom.Services(services)
              .Enrich.FromLogContext()
              .Enrich.WithMachineName()
              .Enrich.WithEnvironmentName());

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddAdminServices(builder.Configuration);
    builder.Services.AddAdminObservability(builder.Configuration);

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
        await seeder.SeedAsync();
    }

    app.UseMiddleware<AdminExceptionHandlingMiddleware>();
    app.UseMiddleware<AdminCorrelationIdMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "BasePlatform Admin v1");
            options.RoutePrefix = "swagger";
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health");

    Log.Information("BasePlatform.Admin started on {Urls}",
        string.Join(", ", builder.WebHost.GetSetting("urls")?.Split(";") ?? ["https://localhost:7135"]));

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Admin application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}