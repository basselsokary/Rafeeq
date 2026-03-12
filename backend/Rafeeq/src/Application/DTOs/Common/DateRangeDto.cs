namespace Application.DTOs.Common;

/// <summary>
/// Date range DTO
/// </summary>
public record DateRangeDto(
    DateTime StartDate,
    DateTime EndDate,
    int DurationInDays);
