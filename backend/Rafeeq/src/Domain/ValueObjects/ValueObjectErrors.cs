using Shared.Models;

namespace Domain.ValueObjects;

public static class OpeningHourErrors
{
    public static Error AlreadyExist(DayOfWeek day) =>
        Error.Conflict("OPENINGHOUR_ALREADY_EXIST", $"Opening hours for {day} already exist.");

    public static Error NotFound(DayOfWeek day) =>
        Error.NotFound(
            "OPENINGHOUR_NOT_FOUND", $"Opening hours for {day} does not exist.");
}

public static class AddressErrors
{
    public static Error EmptyStreet =>
        Error.Validation("ADDRESS_EMPTY_STREET", "Street cannot be empty.");

    public static Error EmptyCity =>
        Error.Validation("ADDRESS_EMPTY_CITY", "City cannot be empty.");
}

public static class DateRangeErrors
{
    public static Error StartDateNotBeforeEndDate =>
        Error.Validation("DATERANGE_INVALID", "Start date must be before end date.");
}

public static class EmailErrors
{
    public static Error Empty =>
        Error.Validation("EMAIL_EMPTY", "Email cannot be empty.");

    public static Error TooLong =>
        Error.Validation("EMAIL_TOO_LONG", "Email is too long.");

    public static Error InvalidFormat(string value) =>
        Error.Validation("EMAIL_INVALID_FORMAT", $"'{value}' is not a valid email address.");
}

public static class GeoLocationErrors
{
    public static Error InvalidLatitude(int latitude) =>
        Error.Validation("GEOLOCATION_INVALID_LATITUDE", $"Latitude must be between -{latitude} and {latitude} degrees.");

    public static Error InvalidLongitude(int longitude) =>
        Error.Validation("GEOLOCATION_INVALID_LONGITUDE", $"Longitude must be between -{longitude} and {longitude} degrees.");
}

public static class MoneyErrors
{
    public static Error NegativeAmount =>
        Error.Validation("MONEY_NEGATIVE_AMOUNT", "Amount cannot be negative.");

    public static Error EmptyCurrency =>
        Error.Validation("MONEY_EMPTY_CURRENCY", "Currency is required.");

    public static Error InvalidCurrencyFormat =>
        Error.Validation("MONEY_INVALID_CURRENCY_FORMAT", "Currency must be a 3-letter ISO code.");

    public static Error CurrencyMismatch =>
        Error.Validation("MONEY_CURRENCY_MISMATCH", "Cannot operate on different currencies.");

    public static Error DivisionByZero =>
        Error.Validation("MONEY_DIVISION_BY_ZERO", "Cannot divide by zero.");
}

public static class PhoneNumberErrors
{
    public static Error Empty =>
        Error.Validation("PHONENUMBER_EMPTY", "Phone number cannot be empty.");

    public static Error InvalidFormat(string value) =>
        Error.Validation("PHONENUMBER_INVALID_FORMAT", $"'{value}' is not a valid phone number.");
}

public static class RatingErrors
{
    public static Error OutOfRange(int min, int max) =>
        Error.Validation("RATING_OUT_OF_RANGE", $"Rating must be between {min} and {max}, and max rating greater than or equal to min rating.");
}

public static class TimeRangeErrors
{
    public static Error StartTimeNotBeforeEndTime =>
        Error.Validation("TIMERANGE_INVALID", "Start time must be before end time.");
}
