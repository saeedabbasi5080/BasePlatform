namespace BasePlatform.Application.Common.Abstractions;

public interface IAuditService
{
    Task LogAsync(
        string action,
        string targetEntityType,
        string targetEntityId,
        string details,
        string? ipAddress = null,
        CancellationToken cancellationToken = default);
}