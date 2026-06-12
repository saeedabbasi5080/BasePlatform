using BasePlatform.Admin.Configuration;
using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Users.AssignRole;
using BasePlatform.Application.Features.Users.DeactivateUser;
using BasePlatform.Application.Features.Users.GetAllUsers;
using BasePlatform.Application.Features.Users.GetUserById;
using BasePlatform.Domain.Constants;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasePlatform.Admin.Controllers;

[ApiController]
[Route("admin/users")]
[Authorize(AuthenticationSchemes = AdminCookieDefaults.AuthenticationScheme)]
public sealed class AdminUsersController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AdminUsersController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    // GET admin/users?page=1&pageSize=20&search=...
    [HttpGet]
    [Authorize(Policy = Permissions.UsersView)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(
            new GetAllUsersQuery(page, pageSize, search), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    // GET admin/users/{id}
    [HttpGet("{id:guid}")]
    [Authorize(Policy = Permissions.UsersView)]
    public async Task<IActionResult> GetUserById(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetUserByIdQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    // POST admin/users/{id}/deactivate
    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Policy = Permissions.UsersDelete)]
    public async Task<IActionResult> DeactivateUser(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new DeactivateUserCommand(id), cancellationToken);

        return result.IsSuccess ? NoContent() : Problem(result);
    }

    // POST admin/users/{id}/assign-role
    [HttpPost("{id:guid}/assign-role")]
    [Authorize(Policy = Permissions.RolesAssign)]
    public async Task<IActionResult> AssignRole(
        Guid id,
        [FromBody] AdminAssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new AssignRoleCommand(id, request.RoleName), cancellationToken);

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

public sealed record AdminAssignRoleRequest(string RoleName);