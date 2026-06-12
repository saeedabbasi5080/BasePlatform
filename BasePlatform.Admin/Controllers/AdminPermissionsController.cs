using BasePlatform.Admin.Configuration;
using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Permissions.AssignPermissionsToRole;
using BasePlatform.Application.Features.Permissions.GetAllPermissions;
using BasePlatform.Application.Features.Permissions.GetRolePermissions;
using BasePlatform.Domain.Constants;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasePlatform.Admin.Controllers;

[ApiController]
[Route("admin/permissions")]
[Authorize(AuthenticationSchemes = AdminCookieDefaults.AuthenticationScheme)]
public sealed class AdminPermissionsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AdminPermissionsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    // GET admin/permissions
    [HttpGet]
    [Authorize(Policy = Permissions.PermissionsView)]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetAllPermissionsQuery(), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    // GET admin/permissions/roles/{roleId}
    [HttpGet("roles/{roleId:guid}")]
    [Authorize(Policy = Permissions.PermissionsView)]
    public async Task<IActionResult> GetRolePermissions(
        Guid roleId, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetRolePermissionsQuery(roleId), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    // PUT admin/permissions/roles/{roleId}
    [HttpPut("roles/{roleId:guid}")]
    [Authorize(Policy = Permissions.PermissionsManage)]
    public async Task<IActionResult> AssignPermissionsToRole(
        Guid roleId,
        [FromBody] AdminAssignPermissionsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new AssignPermissionsToRoleCommand(roleId, request.PermissionIds),
            cancellationToken);

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

public sealed record AdminAssignPermissionsRequest(List<Guid> PermissionIds);