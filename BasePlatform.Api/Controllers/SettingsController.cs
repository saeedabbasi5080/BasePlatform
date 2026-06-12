using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Settings.GetSettingByKey;
using BasePlatform.Application.Features.Settings.GetSettings;
using BasePlatform.Application.Features.Settings.UpsertSetting;
using BasePlatform.Domain.Constants;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasePlatform.Api.Controllers;

[ApiController]
[Route("api/settings")]
public sealed class SettingsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public SettingsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    // GET api/settings/public
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicSettings(
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetSettingsQuery(PublicOnly: true), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    // GET api/settings/{key}
    [HttpGet("{key}")]
    [Authorize]
    [Authorize(Policy = Permissions.SettingsView)]
    public async Task<IActionResult> GetSettingByKey(
        string key, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetSettingByKeyQuery(key), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    // PUT api/settings
    [HttpPut]
    [Authorize(Policy = Permissions.SettingsUpdate)]
    public async Task<IActionResult> UpsertSetting(
        [FromBody] UpsertSettingCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(command, cancellationToken);

        return result.IsSuccess ? NoContent() : Problem(result);
    }

    private IActionResult Problem(Result result) =>
        result.Error.Type switch
        {
            ErrorType.NotFound => NotFound(new { result.Error.Code, result.Error.Description }),
            ErrorType.Unauthorized => Unauthorized(new { result.Error.Code, result.Error.Description }),
            ErrorType.Forbidden => StatusCode(403, new { result.Error.Code, result.Error.Description }),
            ErrorType.Validation => BadRequest(new { result.Error.Code, result.Error.Description }),
            ErrorType.Conflict => Conflict(new { result.Error.Code, result.Error.Description }),
            _ => StatusCode(500, new { result.Error.Code, result.Error.Description })
        };

    private IActionResult Problem<T>(Result<T> result) =>
        result.Error.Type switch
        {
            ErrorType.NotFound => NotFound(new { result.Error.Code, result.Error.Description }),
            ErrorType.Unauthorized => Unauthorized(new { result.Error.Code, result.Error.Description }),
            ErrorType.Forbidden => StatusCode(403, new { result.Error.Code, result.Error.Description }),
            ErrorType.Validation => BadRequest(new { result.Error.Code, result.Error.Description }),
            ErrorType.Conflict => Conflict(new { result.Error.Code, result.Error.Description }),
            _ => StatusCode(500, new { result.Error.Code, result.Error.Description })
        };
}