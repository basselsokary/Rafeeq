using Shared;

namespace Domain.Entities.TripAggregate;

public static class TripErrors
{
    public static Error TouristIdRequired =>
        Error.Validation("TRIP_TOURIST_ID_REQUIRED", "Tourist id is required.");

    public static Error NotFound =>
        Error.NotFound("TRIP_NOT_FOUND", "The specified trip was not found.");

    public static Error SiteNotFound =>
        Error.NotFound("TRIP_SITE_NOT_FOUND", "The specified site was not found in the trip.");
    
    public static Error TripDayNotFound =>
        Error.NotFound("TRIP_DAY_NOT_FOUND", "The specified trip day was not found in the trip.");
    
    public static Error NoteNotFound =>
        Error.NotFound("TRIP_NOTE_NOT_FOUND", "The specified note was not found in the trip.");

    public static Error TitleRequired =>
        Error.Validation("TRIP_NAME_REQUIRED", "Trip title cannot be empty.");

    public static Error SiteNameRequired =>
        Error.Validation("TRIP_SITE_NAME_REQUIRED", "Site name cannot be empty.");

    public static Error CannotBeCompleted =>
        Error.Validation("TRIP_CANNOT_BE_COMPLETED", "Trip cannot be completed in its current state.");
    
    public static Error CannotBeStarted =>
        Error.Validation("TRIP_CANNOT_BE_STARTED", "Trip cannot be started in its current state or start date is in the future.");
    
    public static Error CannotBeCanceled =>
        Error.Validation("TRIP_CANNOT_BE_CANCELED", "Completed trips cannot be canceled.");
    
    public static Error CannotBePublished =>
        Error.Validation("TRIP_CANNOT_BE_PUBLISHED", "Only draft trips can be published.");
    
    public static Error SiteAlreadyMarkedAsVisited =>
        Error.Validation("TRIP_SITE_ALREADY_MARKED_AS_VISITED", "This site has already been marked as visited.");

    public static Error SiteVisitDateOutOfRange =>
        Error.Validation("SITE_VISIT_DATE_OUT_OF_RANGE", "Site visit date must be within the trip date range.");
    
    public static Error InvalidDurationDays(int maxDays) =>
        Error.Validation("INVALID_DURATION_DAYS", $"Trip duration in days must be greater than zero and less than or equal to {maxDays}.");

    public static Error TripDayDateOutOfRange =>
        Error.Validation("TRIP_DAY_DATE_OUT_OF_RANGE", "Trip day date must be within the trip date range.");

    public static Error TripDayAlreadyExists =>
        Error.Conflict("TRIP_DAY_ALREADY_EXISTS", "A trip day for this date already exists.");
    
    public static Error SiteAlreadyAdded =>
        Error.Validation("SITE_ALREADY_ADDED", "This site has already been added to the trip.");

    public static Error EstimatedDurationInvalid =>
        Error.Validation("ESTIMATED_DURATION_INVALID", "Estimated duration must be greater than zero.");

    public static Error DisplayOrderInvalid =>
        Error.Validation("DISPLAY_ORDER_INVALID", "Display order must be zero or a positive number.");

    public static Error DayNumberInvalid =>
        Error.Validation("DAY_NUMBER_INVALID", "Day number must be greater than zero.");
    
    public static Error InvalidBudget =>
        Error.Validation("INVALID_BUDGET", "Invalid budget value.");
}