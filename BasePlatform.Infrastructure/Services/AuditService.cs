using BasePlatform.Application.Common.Abstractions;

namespace BasePlatform.Infrastructure.Services;

public sealed class AuditService : IAuditService
{
    public Task LogAsync(
        string action,
        string targetEntityType,
        string targetEntityId,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        // Full implementation in Phase 15
        return Task.CompletedTask;
    }
}