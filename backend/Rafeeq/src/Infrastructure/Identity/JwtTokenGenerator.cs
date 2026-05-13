using Domain.Enums;
using Infrastructure.Identity.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity;

internal class JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.PreferredUsername, user.UserName!),
        };

        // Add roles as claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Set token expiration based on user role
        var tokenExpiration = roles.Any(r => r.Equals(UserRole.Tourist.ToString(), StringComparison.OrdinalIgnoreCase))
            ? DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes)
            : DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationForAdminInMinutes);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = tokenExpiration,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = false // Don't validate expiration
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        return principal;
    }
}
