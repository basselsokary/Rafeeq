namespace Application.Common.Models;

public class AuthenticationResult : Result
{
    protected AuthenticationResult(bool succeeded, Error error) : base(succeeded, error)
    {
    }

    protected AuthenticationResult(
        bool succeeded,
        Error error,
        string accessToken,
        string refreshToken,
        Guid userId) : base(succeeded, error)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        UserId = userId;
    }

    public string? AccessToken { get; }
    public string? RefreshToken { get; }
    public Guid? UserId { get; }

    public static AuthenticationResult Success(string accessToken, string refreshToken, Guid userId)
    {
        return new AuthenticationResult(true, Error.None, accessToken, refreshToken, userId);
    }

    public new static AuthenticationResult Failure(Error error)
    {
        return new AuthenticationResult(false, error);
    }
}
