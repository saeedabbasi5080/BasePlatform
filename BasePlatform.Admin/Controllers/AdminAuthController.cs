using System.Security.Claims;
using BasePlatform.Admin.Configuration;
using BasePlatform.Domain.Entities;
using BasePlatform.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePlatform.Admin.Controllers;

[ApiController]
[Route("admin/auth")]
public sealed class AdminAuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;

    public AdminAuthController(
        UserManager<AppUser> userManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] AdminLoginRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { Code = "Auth.InvalidInput", Description = "Email and password are required." });

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive)
            return Unauthorized(new { Code = "Auth.InvalidCredentials", Description = "Invalid email or password." });

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            return Unauthorized(new { Code = "Auth.InvalidCredentials", Description = "Invalid email or password." });

        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Unauthorized(new { Code = "Auth.EmailNotConfirmed", Description = "Email is not confirmed." });

        // چک admin.access از RolePermissions — نه AspNetUserClaims
        var hasAdminAccess = await (
            from ur in _context.UserRoles
            join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where ur.UserId == user.Id && p.Name == "admin.access"
            select p.Id
        ).AnyAsync(cancellationToken);

        if (!hasAdminAccess)
            return Unauthorized(new { Code = "Auth.AccessDenied", Description = "You do not have admin access." });

        // گرفتن roles و permissions
        var roles = await _userManager.GetRolesAsync(user);

        var permissions = await (
            from ur in _context.UserRoles
            join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where ur.UserId == user.Id
            select p.Name
        ).Distinct().ToListAsync(cancellationToken);

        // ساخت Cookie
        var claimsList = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.DisplayName)
        };

        claimsList.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        claimsList.AddRange(permissions.Select(p => new Claim("permission", p)));

        var identity = new ClaimsIdentity(claimsList, AdminCookieDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            AdminCookieDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = request.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        return Ok(new
        {
            UserId = user.Id,
            user.Email,
            user.DisplayName,
            Roles = roles,
            Permissions = permissions
        });
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = AdminCookieDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(AdminCookieDefaults.AuthenticationScheme);
        return NoContent();
    }
}

public sealed record AdminLoginRequest(
    string Email,
    string Password,
    bool RememberMe = false);