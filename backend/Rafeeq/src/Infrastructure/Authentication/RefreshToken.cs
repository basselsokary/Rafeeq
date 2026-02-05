namespace Infrastructure.Authentication;

public class RefreshToken
{
    private RefreshToken() { }
    private RefreshToken(string token, string userId, DateTime expiresOn)
    {
        Id = Guid.NewGuid();
        Token = token;
        UserId = userId;
        ExpiresOn = expiresOn;
        IsRevoked = false;
        RevokedAt = default;
    }

    public Guid Id { get; private set; }

    public string Token { get; private set; } = string.Empty;

    public string UserId { get; private set; } = string.Empty;

    public DateTime ExpiresOn { get; private set; }
    public DateTime RevokedAt  { get; private set; }

    public bool IsRevoked { get; private set; }

    public bool IsActive => !IsRevoked && DateTime.UtcNow <= ExpiresOn;


    public static RefreshToken Create(string token, string userId, DateTime expiresOn)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

        if (expiresOn == default)
            throw new ArgumentException("Expiration date must be a valid value.", nameof(expiresOn));

        return new(token, userId, expiresOn);
    }

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }
}