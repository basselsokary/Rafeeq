namespace Application.Common.Models;

public sealed class AuthenticationResult : Result
{
    public string AccessToken { get; } = null!;
    public string RefreshToken { get; } = null!;
    public int AccessTokenExpirationInMinutes { get; }
    public int RefreshTokenExpirationInHours { get; }
    public Guid UserId { get; }

    private AuthenticationResult(bool succeeded, Error error) : base(succeeded, error) { }
    private AuthenticationResult(
        bool succeeded,
        Error error,
        string accessToken,
        string refreshToken,
        int accessTokenExpirationInMinutes,
        int refreshTokenExpirationInHours,
        Guid userId) : base(succeeded, error)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;

        AccessTokenExpirationInMinutes = accessTokenExpirationInMinutes;
        RefreshTokenExpirationInHours = refreshTokenExpirationInHours;

        UserId = userId;
    }

    public static AuthenticationResult Success(
        string accessToken,
        string refreshToken,
        int accessTokenExpirationInMinutes,
        int refreshTokenExpirationInHours,
        Guid userId)
            => new(true, Error.None, accessToken, refreshToken, accessTokenExpirationInMinutes, refreshTokenExpirationInHours, userId);

    public static new AuthenticationResult Failure(Error error)
        => new(false, error);

    public static new AuthenticationResult Failure(string message)
        => new(false, Error.General(message));
}
