using Shared;

namespace Domain.Entities.TouristAggregate;

public class TouristErrors
{
    public static Error NotFound(Guid key) =>
        Error.NotFound("TOURIST_NOT_FOUND", $"The user with ID '{key}' was not found.");
    
    public static Error FavouriteNotFound =>
        Error.NotFound("FAVOURITE_NOT_FOUND", $"Favourite site was not found.");

    public static Error VisitedSiteNotFound =>
        Error.NotFound("VISITED_SITE_NOT_FOUND", $"Visited site was not found.");

    public static Error FirstNameRequired => 
        Error.Validation("TOURIST_FIRST_NAME_REQUIRED", "User first name cannot be empty.");

    public static Error FirstNameExceededLength =>
        Error.Validation("TOURIST_FIRST_NAME_LENGTH_EXCEEDED", "User first name exceeds the maximum allowed length.");
    
    public static Error LastNameRequired => 
        Error.Validation("TOURIST_LAST_NAME_REQUIRED", "User last name cannot be empty.");

    public static Error LastNameExceededLength =>
        Error.Validation("TOURIST_LAST_NAME_LENGTH_EXCEEDED", "User last name exceeds the maximum allowed length.");

    public static Error NationalityRequired => 
        Error.Validation("TOURIST_NATIONALITY_REQUIRED", "User nationality cannot be empty.");

    public static Error ContentRequired =>
        Error.Validation("TRIP_NOTE_CONTENT_REQUIRED", "Trip note content cannot be empty.");
        
    public static Error SiteAlreadyFavorite => 
        Error.Conflict("TOURIST_SITE_ALREADY_FAVORITE", "Site is already in favorites.");
    
    public static Error RequiredSiteId
        => Error.Validation("FAVOURITE_SITE_ID_REQUIRED", "Site ID is required.");

    public static Error VisitDateInFuture
        => Error.Validation("VISITED_SITE_DATE_IN_FUTURE", "Visit date cannot be in the future.");
    
    public static Error DurationInvalid
        => Error.Validation("VISITED_SITE_DURATION_INVALID", "Duration must be greater than zero.");

    public static Error DisplayOrderInvalid
        => Error.Validation("TRIP_SITE_DISPLAY_ORDER_INVALID", "Display order cannot be negative.");

    public static Error SiteAlreadyVisited
        => Error.Conflict("TOURIST_SITE_ALREADY_VISITED", "Site is already marked as visited.");
}