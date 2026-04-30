using Shared;

namespace Infrastructure.Identity.Entities;

public class ModeratorInvitation
{
    private ModeratorInvitation() { }
    private ModeratorInvitation(
        Guid invitedBy,
        string email,
        string token,
        DateTime expiresAt)
    {
        InvitationId = Guid.NewGuid();
        Email = email;
        Token = token;
        InvitedBy = invitedBy;
        ExpiresAt = expiresAt;

        Status = ModeratorInvitationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid InvitationId { get; private set; }
    public string Email { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public Guid InvitedBy { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public ModeratorInvitationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Result<ModeratorInvitation> Create(
        Guid invitedBy,
        string email,
        string token,
        DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ApplicationUserErrors.EmailRequired;

        if (string.IsNullOrWhiteSpace(token))
            return ApplicationUserErrors.TokenRequired;

        return new ModeratorInvitation(invitedBy, email, token, expiresAt);
    }

    public Result Accept()
    {
        if (Status != ModeratorInvitationStatus.Pending)
            return ApplicationUserErrors.InvalidInvitationStatus;

        if (DateTime.UtcNow > ExpiresAt)
        {
            Status = ModeratorInvitationStatus.Expired;
            return ApplicationUserErrors.InvitationExpired;
        }

        Status = ModeratorInvitationStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Revoke()
    {
        if (Status != ModeratorInvitationStatus.Pending)
            return ApplicationUserErrors.InvalidInvitationStatus;

        Status = ModeratorInvitationStatus.Revoked;
        return Result.Success();
    }
    
}

public enum ModeratorInvitationStatus
{
    Pending,
    Accepted,
    Expired,
    Revoked
}
