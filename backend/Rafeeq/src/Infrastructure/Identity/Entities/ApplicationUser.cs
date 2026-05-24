using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Entities;

public abstract class ApplicationUser : IdentityUser<Guid>
{
    protected ApplicationUser() { }
    protected ApplicationUser(Guid userId, string userName, string email, bool mustChangePassword = false)
    {
        Id = userId;
        UserName = userName;
        Email = email;
        MustChangePassword = mustChangePassword;

        CreatedAt = DateTime.UtcNow;
        Status = UserStatus.Active;
        IsActive = true;
    }

    public UserStatus Status { get; protected set; }

    public bool MustChangePassword { get; protected set; }
    public DateTime? LastPasswordChangedAt { get; protected set; }

    public DateTime CreatedAt { get; protected set; }
    public DateTime? LastLoginAt { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public bool IsActive { get; protected set; }

    public bool IsDeleted => DeletedAt.HasValue;
    public bool IsLocked => LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;

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

    public void RequirePasswordChange(bool mustChange = true)
    {
        MustChangePassword = mustChange;
    }

    public void MarkDeleted()
    {
        DeletedAt = DateTime.UtcNow;
        Status = UserStatus.Deleted;
        IsActive = false;
    }
}
