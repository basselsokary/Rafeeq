namespace Infrastructure.Identity;

internal class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpirationInHours { get; set; }
    public int AccessTokenExpirationForAdminInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
    public int RefreshTokenExpirationForAdminInHours { get; set; }
    public int TokenLifespanHours { get; set; }
}