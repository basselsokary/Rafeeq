namespace Application.Common.Interfaces.Authentication;

public interface IExternalIdentityService
{
    Task<string?> ValidateGoogleToken(string idToken);
}