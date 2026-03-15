using Shared.Models;

namespace Domain.Entities.TouristAggregate;

public class TouristErrors
{
    public static Error Unauthorized(string? userId) =>
        Error.Unauthorized("USER_UNAUTHORIZED", $"The user with id = '{userId}' is unauthorized.");

    public static Error NotFound(Guid key) =>
        Error.NotFound("USER_NOT_FOUND", $"The user with = '{key}' was not found.");
    
    public static Error NotFound(string key) =>
        Error.NotFound("USER_NOT_FOUND", $"The user with = '{key}' was not found.");
    
    public static Error FavouriteNotFound(Guid key) =>
        Error.NotFound("FAVOURITE_NOT_FOUND", $"Favourite with ID '{key}' was not found.");

    public static Error FirstNameRequired => 
        Error.Validation("USER_FIRST_NAME_REQUIRED", "User first name cannot be empty.");
    
    public static Error LastNameRequired => 
        Error.Validation("USER_LAST_NAME_REQUIRED", "User last name cannot be empty.");

    public static Error NationalityRequired => 
        Error.Validation("USER_NATIONALITY_REQUIRED", "User nationality cannot be empty.");
    
    public static Error SiteAlreadyFavorite => 
        Error.Conflict("SITE_ALREADY_FAVORITE", "Site is already in favorites.");
    
    public static Error DiscountRequired => 
        Error.Validation("USER_DISCOUNT_REQUIRED", "Either discount amount or percentage must be provided.");

    public static Error InvalidCredentials =>
        Error.Failure("INVALID_CREDENTAILS", "Email or password is wrong.");

    public static Error EmailAlreadyInUse =>
        Error.Conflict("EMAIL_ALREADY_IN_USE", "User with this email already exists.");

    public static Error InActiveUser =>
        Error.Failure("IN_ACTIVE_USER", "Account is deactivated.");

    public static Error LockedAccount =>
        Error.Failure("LOCKED_ACCOUNT", "Account is locked out.");

    public static Error InvalidToken =>
        Error.Failure("INVALID_TOKEN", "Invalid access token.");
    
    public static Error InvalidRefreshToken =>
        Error.Failure("INVALID_REFRESH_TOKEN", "Invalid refresh token.");
    
    public static Error InvalidClaims =>
        Error.Failure("INVALID_CLAIMS", "Invalid token claims.");

    public static Error SomethingWentWrong() =>
        Error.Failure("SOMETHING_WENT_WRONG", "Something went wrong.");

    public static Error RequiredUserId
        => Error.Validation("USER_ID_REQUIRED", "User ID is required.");
}