using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Domain.Entities;

public class AppUserRole : IdentityUserRole<Guid>
{
    public AppUser User { get; set; } = null!;
    public AppRole Role { get; set; } = null!;
}