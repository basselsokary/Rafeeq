using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Entities;

public abstract class ApplicationUser : IdentityUser<Guid>
{
    protected ApplicationUser() { }
    protected ApplicationUser(Guid userId, string userName, string email, UserRole role)
    {
        Id = userId;
        UserName = userName;
        Email = email;
        Role = role;

        CreatedAt = DateTime.UtcNow;
        Status = UserStatus.Active;
        IsActive = true;
    }

    public UserRole Role { get; protected set; }
    public UserStatus Status { get; protected set; }

    public DateTime CreatedAt { get; protected set; }
    public DateTime? LastLoginAt { get; protected set; }
    public bool IsActive { get; protected set; }

    public void UpdateRole(UserRole newRole)
    {
        Role = newRole;
    }

    public void RecordLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void SuspendAccount()
    {
        Status = UserStatus.Suspended;
        IsActive = false;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        IsActive = true;
    }
}
