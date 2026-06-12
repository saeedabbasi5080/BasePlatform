using System.Security.Claims;
using BasePlatform.Domain.Entities;
using BasePlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BasePlatform.Infrastructure.Authorization;

public sealed class PermissionClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<AppUser, AppRole>
{
    private readonly AppDbContext _context;

    public PermissionClaimsPrincipalFactory(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor,
        AppDbContext context)
        : base(userManager, roleManager, optionsAccessor)
    {
        _context = context;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        var roleNames = await UserManager.GetRolesAsync(user);

        var permissions = await _context.RolePermissions
            .AsNoTracking()
            .Where(rp => roleNames.Contains(rp.Role.Name!))
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync();

        foreach (var permission in permissions)
            identity.AddClaim(new Claim("permission", permission));

        return identity;
    }
}