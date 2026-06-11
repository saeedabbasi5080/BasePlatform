using BasePlatform.Domain.Entities;
using BasePlatform.Infrastructure.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BasePlatform.Infrastructure.Identity;

public class IdentitySeeder
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentitySeeder> _logger;

    public IdentitySeeder(
        RoleManager<AppRole> roleManager,
        UserManager<AppUser> userManager,
        IConfiguration configuration,
        ILogger<IdentitySeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await RoleSeeder.SeedAsync(_roleManager, _logger);
        await AdminUserSeeder.SeedAsync(_userManager, _configuration, _logger);
    }
}