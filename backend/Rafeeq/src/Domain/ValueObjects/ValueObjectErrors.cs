using Shared;

namespace Domain.ValueObjects;

public static class OpeningHourErrors
{
    public static Error AlreadyExist(DayOfWeek day) =>
        Error.Conflict("OPENING_HOURS_ALREADY_EXIST", $"Opening hours for {day} already exist.");

    public static Error NotFound(DayOfWeek day) =>
        Error.NotFound(
            "OPENING_HOURS_NOT_FOUND", $"Opening hours for {day} do not exist.");
}

public static class AddressErrors
{
    public static Error EmptyAddress =>
        Error.Validation("ADDRESS_REQUIRED", "Address is required.");

    public static Error ExceededAddressLength =>
        Error.Validation("ADDRESS_LENGTH_EXCEEDED", "Address length cannot exceed the defined maximum.");

    public static Error ExceededStreetLength =>
        Error.Validation("ADDRESS_STREET_LENGTH_EXCEEDED", "Street length cannot exceed the defined maximum.");

    public static Error ExceededCityLength =>
        Error.Validation("ADDRESS_CITY_LENGTH_EXCEEDED", "City length cannot exceed the defined maximum.");

    public static Error ExceededRegionLength =>
        Error.Validation("ADDRESS_REGION_LENGTH_EXCEEDED", "Region length cannot exceed the defined maximum.");
    
    public static Error ExceededPostalCodeLength =>
        Error.Validation("ADDRESS_POSTAL_CODE_LENGTH_EXCEEDED", "Postal code length cannot exceed the defined maximum.");

    public static Error EmptyStreet =>
        Error.Validation("ADDRESS_STREET_REQUIRED", "Street is required.");

    public static Error EmptyCity =>
        Error.Validation("ADDRESS_CITY_REQUIRED", "City is required.");
}

public static class DateRangeErrors
{
    public static Error StartDateNotBeforeEndDate =>
    Error.Validation("DATE_RANGE_INVALID", "Start date must be before end date.");
}

public static class EmailErrors
{
    public static Error Empty =>
        Error.Validation("EMAIL_REQUIRED", "Email is required.");

    public static Error TooLong =>
        Error.Validation("EMAIL_TOO_LONG", "Email is too long.");

    public static Error InvalidFormat(string value) =>
        Error.Validation("EMAIL_INVALID_FORMAT", $"'{value}' is not a valid email address.");
}

public static class GeoLocationErrors
{
    public static Error InvalidLatitude(double latitude) =>
        Error.Validation("GEOLOCATION_INVALID_LATITUDE", $"Latitude must be between -90 and 90 degrees. Received: {latitude}.");

    public static Error InvalidLongitude(double longitude) =>
        Error.Validation("GEOLOCATION_INVALID_LONGITUDE", $"Longitude must be between -180 and 180 degrees. Received: {longitude}.");
}

public static class MoneyErrors
{
    public static Error NegativeAmount =>
        Error.Validation("MONEY_NEGATIVE_AMOUNT", "Amount cannot be negative.");

    public static Error EmptyCurrency =>
        Error.Validation("MONEY_CURRENCY_REQUIRED", "Currency is required.");

    public static Error InvalidCurrencyFormat =>
        Error.Validation("MONEY_INVALID_CURRENCY_FORMAT", "Currency must be a 3-letter ISO code.");

    public static Error CurrencyMismatch =>
        Error.Validation("MONEY_CURRENCY_MISMATCH", "Cannot operate on different currencies.");

    public static Error DivisionByZero =>
        Error.Validation("MONEY_DIVISION_BY_ZERO", "Cannot divide by zero.");
}

public static class TicketErrors
{
    public static Error ExceededNotesLength =>
        Error.Validation("TICKET_NOTES_LENGTH_EXCEEDED", "Ticket notes length cannot exceed the defined maximum.");
}

public static class PhoneNumberErrors
{
    public static Error Empty =>
        Error.Validation("PHONE_NUMBER_REQUIRED", "Phone number is required.");

    public static Error InvalidFormat(string value) =>
        Error.Validation("PHONENUMBER_INVALID_FORMAT", $"'{value}' is not a valid phone number.");
}

public static class RatingErrors
{
    public static Error OutOfRange(int min, int max) =>
        Error.Validation("RATING_OUT_OF_RANGE", $"Rating must be between {min} and {max}, and max rating greater than or equal to min rating.");

    public static Error PositiveRatingRequired =>
        Error.Validation("RATING_POSITIVE_REQUIRED", "Rating must be a positive integer.");
}

public static class TimeRangeErrors
{
    public static Error StartTimeNotBeforeEndTime =>
    Error.Validation("TIME_RANGE_INVALID", "Start time must be before end time.");
}
