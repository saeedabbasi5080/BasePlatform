using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasePlatform.Infrastructure.Persistence.Repositories;

public sealed class SettingRepository : ISettingRepository
{
    private readonly AppDbContext _context;

    public SettingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AppSetting?> GetByKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        return await _context.AppSettings
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }

    public async Task UpsertAsync(
        AppSetting setting,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.AppSettings
            .FirstOrDefaultAsync(s => s.Id == setting.Id, cancellationToken);

        if (existing is null)
            await _context.AppSettings.AddAsync(setting, cancellationToken);
        else
            _context.AppSettings.Update(setting);

        await _context.SaveChangesAsync(cancellationToken);
    }
}