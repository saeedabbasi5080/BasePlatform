using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Audit.GetAuditLogs;
using BasePlatform.Domain.Constants;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasePlatform.Api.Controllers;

[ApiController]
[Route("api/audit")]
[Authorize]
public sealed class AuditController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AuditController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    // GET api/audit?page=1&pageSize=20&actorEmail=...&action=...
    [HttpGet]
    [Authorize(Policy = Permissions.AuditView)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? actorEmail = null,
        [FromQuery] string? action = null,
        [FromQuery] string? targetEntityType = null,
        [FromQuery] DateTimeOffset? from = null,
        [FromQuery] DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuditLogsQuery(
            page,
            pageSize,
            actorEmail,
            action,
            targetEntityType,
            from,
            to);

        var result = await _dispatcher.QueryAsync(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    private IActionResult Problem<T>(Result<T> result) =>
        result.Error.Type switch
        {
            ErrorType.NotFound => NotFound(new { result.Error.Code, result.Error.Description }),
            ErrorType.Unauthorized => Unauthorized(new { result.Error.Code, result.Error.Description }),
            ErrorType.Forbidden => StatusCode(403, new { result.Error.Code, result.Error.Description }),
            ErrorType.Validation => BadRequest(new { result.Error.Code, result.Error.Description }),
            _ => StatusCode(500, new { result.Error.Code, result.Error.Description })
        };
}