using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Domain.Entities;

public class AppRole : IdentityRole<Guid>
{
    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}