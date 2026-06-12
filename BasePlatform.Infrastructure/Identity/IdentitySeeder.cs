using BasePlatform.Domain.Entities;
using BasePlatform.Infrastructure.Persistence;
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
    private readonly AppDbContext _context;

    public IdentitySeeder(
        RoleManager<AppRole> roleManager,
        UserManager<AppUser> userManager,
        IConfiguration configuration,
        ILogger<IdentitySeeder> logger,
        AppDbContext context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _context = context;
    }

    public async Task SeedAsync()
    {
        await RoleSeeder.SeedAsync(_roleManager, _logger);
        await PermissionSeeder.SeedAsync(_context, _logger);
        await AdminUserSeeder.SeedAsync(_userManager, _configuration, _logger);
        await SettingsSeeder.SeedAsync(_context, _logger);
    }
}