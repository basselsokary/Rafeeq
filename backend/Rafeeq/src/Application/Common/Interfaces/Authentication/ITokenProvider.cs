namespace Application.Common.Interfaces.Authentication;

public interface ITokenProvider
{
    string GenerateAccessToken(string userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
}
