namespace Application.Common.Models;

public sealed class AuthenticationResult : Result
{
    public string AccessToken { get; } = null!;
    public string RefreshToken { get; } = null!;
    public int AccessTokenExpiresAtInHours { get; }
    public int RefreshTokenExpiresAtInDays { get; }
    public Guid UserId { get; }

    private AuthenticationResult(bool succeeded, Error error) : base(succeeded, error) { }
    private AuthenticationResult(
        bool succeeded,
        Error error,
        string accessToken,
        string refreshToken,
        int accessTokenExpiresAt,
        int refreshTokenExpiresAt,
        Guid userId) : base(succeeded, error)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;

        AccessTokenExpiresAtInHours = accessTokenExpiresAt;
        RefreshTokenExpiresAtInDays = refreshTokenExpiresAt;

        UserId = userId;
    }

    public static AuthenticationResult Success(
        string accessToken,
        string refreshToken,
        int accessTokenExpiresAtInHours,
        int refreshTokenExpiresAtInDays,
        Guid userId)
            => new(true, Error.None, accessToken, refreshToken, accessTokenExpiresAtInHours, refreshTokenExpiresAtInDays, userId);

    public static new AuthenticationResult Failure(Error error)
        => new(false, error);

    public static new AuthenticationResult Failure(string message)
        => new(false, Error.General(message));
}
