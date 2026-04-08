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

    public static Error RequiredUserId
        => Error.Validation("USER_ID_REQUIRED", "User ID is required.");
}