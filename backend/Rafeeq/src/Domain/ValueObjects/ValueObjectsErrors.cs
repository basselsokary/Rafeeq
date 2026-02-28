using Shared.Models;

namespace Domain.ValueObjects;

public static class OpeningHourErrors
{
    public static Error AlreadyExist(DayOfWeek day) =>
        Error.Conflict(
            "OPENINGHOUR_ALREADY_EXIST",
            $"Opening hours for {day} already exist.");
    
    public static Error NotFound(DayOfWeek day) =>
        Error.Conflict(
            "OPENINGHOUR_NOT_FOUND",
            $"Opening hours for {day} does not exist.");
}
