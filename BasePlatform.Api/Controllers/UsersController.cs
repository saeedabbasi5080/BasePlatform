using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Users.AssignRole;
using BasePlatform.Application.Features.Users.ChangePassword;
using BasePlatform.Application.Features.Users.DeactivateUser;
using BasePlatform.Application.Features.Users.GetAllUsers;
using BasePlatform.Application.Features.Users.GetCurrentUser;
using BasePlatform.Application.Features.Users.GetUserById;
using BasePlatform.Application.Features.Users.UpdateProfile;
using BasePlatform.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasePlatform.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public UsersController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    // GET api/users/me
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetCurrentUserQuery(), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    // GET api/users/{id}
    [HttpGet("{id:guid}")]
    [Authorize(Policy = Permissions.UsersView)]
    public async Task<IActionResult> GetUserById(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetUserByIdQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    // GET api/users?page=1&pageSize=20&search=...
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

    // PUT api/users/me/profile
    [HttpPut("me/profile")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result);
    }

    // POST api/users/me/change-password
    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result);
    }

    // POST api/users/{id}/deactivate
    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Policy = Permissions.UsersDelete)]
    public async Task<IActionResult> DeactivateUser(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new DeactivateUserCommand(id), cancellationToken);

        return result.IsSuccess ? NoContent() : Problem(result);
    }

    // POST api/users/{id}/assign-role
    [HttpPost("{id:guid}/assign-role")]
    [Authorize(Policy = Permissions.RolesAssign)]
    public async Task<IActionResult> AssignRole(
        Guid id,
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new AssignRoleCommand(id, request.RoleName), cancellationToken);

        return result.IsSuccess ? NoContent() : Problem(result);
    }

    private IActionResult Problem(BasePlatform.Shared.Result result) =>
        result.Error.Type switch
        {
            BasePlatform.Shared.ErrorType.NotFound => NotFound(new { result.Error.Code, result.Error.Description }),
            BasePlatform.Shared.ErrorType.Unauthorized => Unauthorized(new { result.Error.Code, result.Error.Description }),
            BasePlatform.Shared.ErrorType.Forbidden => StatusCode(403, new { result.Error.Code, result.Error.Description }),
            BasePlatform.Shared.ErrorType.Validation => BadRequest(new { result.Error.Code, result.Error.Description }),
            BasePlatform.Shared.ErrorType.Conflict => Conflict(new { result.Error.Code, result.Error.Description }),
            _ => StatusCode(500, new { result.Error.Code, result.Error.Description })
        };

    private IActionResult Problem<T>(BasePlatform.Shared.Result<T> result) =>
        result.Error.Type switch
        {
            BasePlatform.Shared.ErrorType.NotFound => NotFound(new { result.Error.Code, result.Error.Description }),
            BasePlatform.Shared.ErrorType.Unauthorized => Unauthorized(new { result.Error.Code, result.Error.Description }),
            BasePlatform.Shared.ErrorType.Forbidden => StatusCode(403, new { result.Error.Code, result.Error.Description }),
            BasePlatform.Shared.ErrorType.Validation => BadRequest(new { result.Error.Code, result.Error.Description }),
            BasePlatform.Shared.ErrorType.Conflict => Conflict(new { result.Error.Code, result.Error.Description }),
            _ => StatusCode(500, new { result.Error.Code, result.Error.Description })
        };
}

public sealed record AssignRoleRequest(string RoleName);