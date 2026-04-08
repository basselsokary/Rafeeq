using Microsoft.AspNetCore.Identity;
using Shared.Models;

namespace Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    private ApplicationUser() { }
    private ApplicationUser(Guid userId, string userName, string email)
    {
        Id = userId;
        UserName = userName;
        Email = email;

        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; }

    public static Result<ApplicationUser> Create(Guid userId, string userName, string email)
    {
        return new ApplicationUser(userId, userName, email);
    }

    public void RecordLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}

public static class ApplicationUserErrors
{
    public static Error NotFound(string key)
        => Error.NotFound(
            "USER_NOT_FOUND", $"User with '{key}' was not found.");
    
    public static Error NotFound(Guid id)
        => Error.NotFound(
            "USER_NOT_FOUND", $"User with ID '{id}' was not found.");

    public static Error RefreshTokenRequired
        => Error.Validation(
            "REFRESH_TOKEN_REQUIRED", "A valid refresh token is required to refresh the access token.");

    public static Error RefreshTokenExpirationInvalid
        => Error.Validation(
            "REFRESH_TOKEN_EXPIRATION_INVALID", "The provided refresh token has expired.");

    public static Error InvalidCredentials =>
        Error.Failure(
            "INVALID_CREDENTAILS", "Email or password is wrong.");

    public static Error EmailAlreadyInUse =>
        Error.Conflict(
            "EMAIL_ALREADY_IN_USE", "User with this email already exists.");

    public static Error InActiveUser =>
        Error.Failure(
            "IN_ACTIVE_USER", "Account is deactivated.");

    public static Error LockedAccount =>
        Error.Failure(
            "LOCKED_ACCOUNT", "Account is locked out.");

    public static Error InvalidToken =>
        Error.Failure(
            "INVALID_TOKEN", "Invalid access token.");
    
    public static Error InvalidRefreshToken =>
        Error.Failure(
            "INVALID_REFRESH_TOKEN", "Invalid or expired refresh token.");
    
    public static Error InvalidClaims =>
        Error.Failure(
            "INVALID_CLAIMS", "Invalid token claims.");

    public static Error EmailNotConfirmed =>
        Error.Failure(
            "EMAIL_NOT_CONFIRMED", "Email address is not confirmed.");

}