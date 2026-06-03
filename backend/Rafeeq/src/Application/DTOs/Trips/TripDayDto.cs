using Application.DTOs.Common;

namespace Application.DTOs.Trips;

public sealed record TripDayDto(
    Guid Id,
    int DayNumber,
    DateTime Date,
    MoneyDto? EstimatedDayCost,
    int TotalSites,
    int TotalDurationMinutes,
    string? Notes,
    List<TripSiteDto> Sites);
