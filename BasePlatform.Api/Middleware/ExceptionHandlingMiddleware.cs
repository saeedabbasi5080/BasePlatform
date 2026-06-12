using System.Net;
using System.Text.Json;

namespace BasePlatform.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}",
                context.TraceIdentifier);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, code, message) = exception switch
        {
            ArgumentNullException => (
                HttpStatusCode.BadRequest,
                "Request.NullArgument",
                "A required argument was null."),

            ArgumentException => (
                HttpStatusCode.BadRequest,
                "Request.InvalidArgument",
                exception.Message),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Auth.Unauthorized",
                "Unauthorized."),

            _ => (
                HttpStatusCode.InternalServerError,
                "Server.Error",
                "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            Code = code,
            Description = message,
            TraceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}