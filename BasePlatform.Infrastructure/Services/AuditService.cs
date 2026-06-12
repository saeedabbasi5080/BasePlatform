using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Infrastructure.Persistence;

namespace BasePlatform.Infrastructure.Services;

public sealed class AuditService : IAuditService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuditService(
        AppDbContext context,
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task LogAsync(
        string action,
        string targetEntityType,
        string targetEntityId,
        string details,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorId = _currentUser.UserId,
            ActorEmail = _currentUser.Email ?? "system",
            Action = action,
            TargetEntityType = targetEntityType,
            TargetEntityId = targetEntityId,
            Details = details,
            IpAddress = ipAddress ?? string.Empty,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _context.AuditLogs.AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}