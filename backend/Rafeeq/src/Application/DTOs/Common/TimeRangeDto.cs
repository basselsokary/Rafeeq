namespace Application.DTOs.Common;

/// <summary>
/// Time range DTO
/// </summary>
public record TimeRangeDto(
    string StartTime, // "09:00"
    string EndTime,   // "17:00"
    int DurationMinutes);
