using Infrastructure.Identity.Entities;
using Shared;

namespace Infrastructure.Authentication;

public class RefreshToken
{
    private RefreshToken() { }
    private RefreshToken(string token, Guid userId, DateTime expiresAt)
    {
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;

        CreatedAt = DateTime.UtcNow;
        IsRevoked = false;
    }

    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsActive => !IsRevoked && (DateTime.UtcNow < ExpiresAt);

    public static Result<RefreshToken> Create(string token, Guid userId, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            return ApplicationUserErrors.RefreshTokenRequired;

        if (expiresAt <= DateTime.UtcNow)
            return ApplicationUserErrors.RefreshTokenExpirationInvalid;
        
        return new RefreshToken(token, userId, expiresAt);
    }

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }
}
