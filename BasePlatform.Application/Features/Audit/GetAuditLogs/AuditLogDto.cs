namespace BasePlatform.Application.Features.Audit.GetAuditLogs;

public sealed record AuditLogDto(
    Guid Id,
    Guid? ActorId,
    string ActorEmail,
    string Action,
    string TargetEntityType,
    string TargetEntityId,
    string Details,
    string IpAddress,
    DateTimeOffset CreatedAt);