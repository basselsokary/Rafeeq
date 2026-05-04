using Shared;

namespace Domain.Entities.TripAggregate;

public static class TripErrors
{
    public static Error NotFound =>
        Error.NotFound("TRIP_NOT_FOUND", "The specified trip was not found.");

    public static Error SiteNotFound =>
        Error.NotFound("TRIP_SITE_NOT_FOUND", "The specified site was not found in the trip.");
    
    public static Error NoteNotFound =>
        Error.NotFound("TRIP_NOTE_NOT_FOUND", "The specified note was not found in the trip.");

    public static Error NameRequired =>
        Error.Validation("TRIP_NAME_REQUIRED", "Trip name cannot be empty.");

    public static Error CannotBeCompleted =>
        Error.Validation("TRIP_CANNOT_BE_COMPLETED", "Trip cannot be completed in its current state.");
    
    public static Error CannotBeStarted =>
        Error.Validation("TRIP_CANNOT_BE_STARTED", "Trip cannot be started in its current state or start date is in the future.");
    
    public static Error CannotBeCanceled =>
        Error.Validation("TRIP_CANNOT_BE_CANCELED", "Completed trips cannot be canceled.");
    
    public static Error CannotBePublished =>
        Error.Validation("TRIP_CANNOT_BE_PUBLISHED", "Draft trips cannot be published.");
    
    public static Error SiteAlreadyMarkedAsVisited =>
        Error.Validation("TRIP_SITE_ALREADY_MARKED_AS_VISITED", "This site has already been marked as visited.");

    public static Result SiteVisitDateOutOfRange =>
        Error.Validation("SITE_VISIT_DATE_OUT_OF_RANGE", "Site visit date must be within the trip date range.");
    
    public static Result SiteAlreadyAdded =>
        Error.Validation("SITE_ALREADY_ADDED", "This site has already been added to the trip.");

    public static Result EstimatedDurationInvalid =>
        Error.Validation("ESTIMATED_DURATION_INVALID", "Estimated duration must be greater than zero.");
}