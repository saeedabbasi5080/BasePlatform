using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Admin.Auth.Login;
using BasePlatform.Domain.Entities;
using BasePlatform.Infrastructure.Persistence;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BasePlatform.Infrastructure.Queries.Admin;

public sealed class AdminLoginCommandHandler
    : ICommandHandler<AdminLoginCommand, Result<AdminLoginResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppDbContext _context;

    public AdminLoginCommandHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task<Result<AdminLoginResponse>> HandleAsync(
        AdminLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        // ۱. پیدا کردن user
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null || !user.IsActive)
            return Result<AdminLoginResponse>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));

        // ۲. چک کردن password
        var passwordValid = await _userManager.CheckPasswordAsync(user, command.Password);
        if (!passwordValid)
            return Result<AdminLoginResponse>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));


        // DEBUG — موقت
        var userRoleCount = await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .CountAsync(cancellationToken);

        var rolePermCount = await _context.RolePermissions.CountAsync(cancellationToken);
        var permCount = await _context.Permissions.CountAsync(cancellationToken);

        throw new Exception($"DEBUG: UserRoles={userRoleCount}, RolePerms={rolePermCount}, Perms={permCount}, UserId={user.Id}");

        // ۳. چک admin.access — مستقیم از جداول بدون navigation
        var hasAdminAccess = await (
            from ur in _context.UserRoles
            join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where ur.UserId == user.Id && p.Name == "admin.access"
            select p.Id
        ).AnyAsync(cancellationToken);

        if (!hasAdminAccess)
            return Result<AdminLoginResponse>.Failure(
                Error.Forbidden("Auth.AccessDenied", "You do not have admin access."));

        // ۴. Sign in با Cookie
        await _signInManager.SignInAsync(user, isPersistent: command.RememberMe);

        // ۵. گرفتن roles
        var roles = await _userManager.GetRolesAsync(user);

        // ۶. گرفتن permissions
        var permissions = await (
            from ur in _context.UserRoles
            join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where ur.UserId == user.Id
            select p.Name
        ).Distinct().ToListAsync(cancellationToken);

        return Result<AdminLoginResponse>.Success(new AdminLoginResponse(
            UserId: user.Id,
            Email: user.Email!,
            DisplayName: user.DisplayName,
            Roles: roles.ToList(),
            Permissions: permissions));
    }
}