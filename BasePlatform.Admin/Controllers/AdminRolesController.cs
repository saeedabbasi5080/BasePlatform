using BasePlatform.Admin.Configuration;
using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Roles.CreateRole;
using BasePlatform.Application.Features.Roles.DeleteRole;
using BasePlatform.Application.Features.Roles.GetAllRoles;
using BasePlatform.Application.Features.Roles.GetRoleById;
using BasePlatform.Application.Features.Roles.UpdateRole;
using BasePlatform.Domain.Constants;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasePlatform.Admin.Controllers;

[ApiController]
[Route("admin/roles")]
[Authorize(AuthenticationSchemes = AdminCookieDefaults.AuthenticationScheme)]
public sealed class AdminRolesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AdminRolesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    // GET admin/roles
    [HttpGet]
    [Authorize(Policy = Permissions.RolesView)]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetAllRolesQuery(), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    // GET admin/roles/{id}
    [HttpGet("{id:guid}")]
    [Authorize(Policy = Permissions.RolesView)]
    public async Task<IActionResult> GetRoleById(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetRoleByIdQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    // POST admin/roles
    [HttpPost]
    [Authorize(Policy = Permissions.RolesCreate)]
    public async Task<IActionResult> CreateRole(
        [FromBody] CreateRoleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetRoleById), new { id = result.Value }, null)
            : Problem(result);
    }

    // PUT admin/roles/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Policy = Permissions.RolesEdit)]
    public async Task<IActionResult> UpdateRole(
        Guid id,
        [FromBody] AdminUpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new UpdateRoleCommand(id, request.Name, request.Description),
            cancellationToken);

        return result.IsSuccess ? NoContent() : Problem(result);
    }

    // DELETE admin/roles/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Permissions.RolesDelete)]
    public async Task<IActionResult> DeleteRole(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new DeleteRoleCommand(id), cancellationToken);

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

public sealed record AdminUpdateRoleRequest(string Name, string Description);