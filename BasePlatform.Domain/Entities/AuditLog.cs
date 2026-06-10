namespace BasePlatform.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid? ActorId { get; set; }
    public string ActorEmail { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string TargetEntityType { get; set; } = string.Empty;
    public string TargetEntityId { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}