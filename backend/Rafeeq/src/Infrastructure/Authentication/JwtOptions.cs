namespace Infrastructure.Authentication;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpirationInMinutes { get; set; }
    public int AccessTokenExpirationForAdminInMinutes { get; set; }
    public int RefreshTokenExpirationInHours { get; set; }
    public int RefreshTokenExpirationForAdminInHours { get; set; }
    public int TokenLifespanHours { get; set; }
}