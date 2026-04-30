using Application.Common.Interfaces.Authentication;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ExternalServices.GoogleService;

internal class ExternalIdentityService(
    IConfiguration configuration,
    ILogger<ExternalIdentityService> logger) : IExternalIdentityService
{
    public async Task<string?> ValidateGoogleToken(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [configuration["Authentication:Google:ClientId"]]
            });
            
            return payload.Email;
        }
        catch (InvalidJwtException)
        {
            logger.LogWarning("Invalid Google token provided.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during Google authentication.");
            throw new InvalidOperationException("An error occurred during Google authentication.");
        }
    }
}
