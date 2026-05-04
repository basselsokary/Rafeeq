using Shared;

namespace Infrastructure.Identity.Entities;

public static class ApplicationUserErrors
{
    public static Error FullNameRequired =>
        Error.Validation("FULL_NAME_REQUIRED", "Full name is required.");
    public static Error FirstNameRequired =>
        Error.Validation("FIRST_NAME_REQUIRED", "First name is required.");
    public static Error LastNameRequired =>
        Error.Validation("LAST_NAME_REQUIRED", "Last name is required.");

    public static Error NotFound(string key)
        => Error.NotFound("USER_NOT_FOUND", $"User with '{key}' was not found.");

    public static Error NotFound(Guid id)
        => Error.NotFound("USER_NOT_FOUND", $"User with ID '{id}' was not found.");

    public static Error UserNameRequired =>
        Error.Validation("USERNAME_REQUIRED", "Username is required.");

    public static Error EmailRequired =>
        Error.Validation("EMAIL_REQUIRED", "Email is required.");

    public static Error RefreshTokenRequired
        => Error.Validation("REFRESH_TOKEN_REQUIRED", "A valid refresh token is required to refresh the access token.");

    public static Error RefreshTokenExpirationInvalid
        => Error.Validation("REFRESH_TOKEN_EXPIRATION_INVALID", "The provided refresh token has expired.");

    public static Error InvalidCredentials =>
        Error.Failure("INVALID_CREDENTIALS", "Email or password is wrong.");

    public static Error EmailAlreadyInUse =>
        Error.Conflict("EMAIL_ALREADY_IN_USE", "User with this email already exists.");

    public static Error UserNameAlreadyInUse =>
        Error.Conflict("USERNAME_ALREADY_IN_USE", "User with this username already exists.");

    public static Error InActiveUser =>
        Error.Failure("IN_ACTIVE_USER", "Account is deactivated.");

    public static Error LockedAccount =>
        Error.Failure("LOCKED_ACCOUNT", "Account is locked out.");

    public static Error InvalidToken =>
        Error.Failure("INVALID_TOKEN", "Invalid access token.");
    
    public static Error TokenRequired =>
        Error.Failure("TOKEN_REQUIRED", "Access token is required.");

    public static Error InvalidRefreshToken =>
        Error.Failure("INVALID_REFRESH_TOKEN", "Invalid or expired refresh token.");

    public static Error InvalidClaims =>
        Error.Failure("INVALID_CLAIMS", "Invalid token claims.");

    public static Error EmailNotConfirmed =>
        Error.Failure("EMAIL_NOT_CONFIRMED", "Email address is not confirmed.");

    public static Error InvalidInvitationStatus =>
        Error.Failure("INVALID_INVITATION_STATUS", "Only pending invitations can be accepted.");

    public static Error InvitationExpired =>
        Error.Failure("INVITATION_EXPIRED", "This invitation has expired.");
}