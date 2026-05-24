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

    public static Error MustChangePassword =>
        Error.Failure("MUST_CHANGE_PASSWORD", "User must change their password before logging in.");

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

    public static Error RolesRequired =>
        Error.Validation("USER_ROLES_REQUIRED", "At least one role is required.");

    public static Error AlreadyAdmin =>
        Error.Failure("USER_ALREADY_ADMIN", "User is already an admin.");

    public static Error NotModerator =>
        Error.Failure("USER_NOT_MODERATOR", "User must be a moderator before being promoted to admin.");

    public static Error NotAdmin =>
        Error.Failure("USER_NOT_ADMIN", "User is not an admin.");

    public static Error AdminLockNotAllowed =>
        Error.Forbidden("USER_ADMIN_LOCK_FORBIDDEN", "Cannot lock admin accounts.");

    public static Error AdminDeleteNotAllowed =>
        Error.Forbidden("USER_ADMIN_DELETE_FORBIDDEN", "Cannot delete admin accounts. Demote to moderator first.");

    public static Error DeletionConfirmationRequired =>
        Error.Validation("USER_DELETE_CONFIRM_REQUIRED", "Deletion confirmation is required.");

    public static Error PromotionFailed(string details) =>
        Error.Failure("USER_PROMOTION_FAILED", $"Failed to promote user: {details}");

    public static Error DemotionFailed(string details) =>
        Error.Failure("USER_DEMOTION_FAILED", $"Failed to demote user: {details}");

    public static Error RemoveRolesFailed(string details) =>
        Error.Failure("USER_REMOVE_ROLES_FAILED", $"Failed to remove roles: {details}");

    public static Error AddRolesFailed(string details) =>
        Error.Failure("USER_ADD_ROLES_FAILED", $"Failed to add roles: {details}");

    public static Error LockFailed(string details) =>
        Error.Failure("USER_LOCK_FAILED", $"Failed to lock account: {details}");

    public static Error UnlockFailed(string details) =>
        Error.Failure("USER_UNLOCK_FAILED", $"Failed to unlock account: {details}");

    public static Error SuspendFailed(string details) =>
        Error.Failure("USER_SUSPEND_FAILED", $"Failed to suspend account: {details}");

    public static Error ReactivateFailed(string details) =>
        Error.Failure("USER_REACTIVATE_FAILED", $"Failed to reactivate account: {details}");

    public static Error DeleteFailed(string details) =>
        Error.Failure("USER_DELETE_FAILED", $"Failed to delete user: {details}");

    public static Error PermanentDeleteFailed(string details) =>
        Error.Failure("USER_PERMANENT_DELETE_FAILED", $"Failed to delete user: {details}");
}