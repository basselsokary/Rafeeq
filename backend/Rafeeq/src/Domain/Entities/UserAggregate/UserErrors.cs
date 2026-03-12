using Shared.Models;

namespace Domain.Entities.UserAggregate;

public class UserErrors
{
    public static Error Unauthorized(string? userId) =>
        Error.Unauthorized("USER_UNAUTHORIZED", $"The user with id = '{userId}' is unauthorized.");

    public static Error NotFound(Guid key) =>
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

    public static Error InvalidCredentials() =>
        Error.Failure("INVALID_CREDENTAILS", "Email or password is wrong.");

    public static Error EmailAlreadyInUse(string email) =>
    Error.Conflict("EMAIL_ALREADY_IN_USE", $"Email {email} already in use.");
    
    public static Error SomethingWentWrong() =>
        Error.Failure("SOMETHING_WENT_WRONG", "Something went wrong.");
}