namespace BasePlatform.Domain.Entities;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public AppRole Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}