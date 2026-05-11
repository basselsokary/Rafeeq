namespace Application.DTOs.Common;

public record TimeRangeDto(
    TimeOnly StartTime,
    TimeOnly EndTime,
    int DurationMinutes);
