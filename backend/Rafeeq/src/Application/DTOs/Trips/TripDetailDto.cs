using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Trips;

public sealed record TripDetailDto(
    Guid Id,
    string Title,
    string? Description,
    DateOnly StartDate,
    DateOnly EndDate,
    TimeOnly DailyStartTime,
    TimeOnly DailyEndTime,
    LocationDto UserPosition,
    TripStatus Status,
    Tolerance? Tolerance,
    MoneyDto? EstimatedBudget,
    MoneyDto? ActualCost,
    int TotalSites,
    int EstimatedTotalDurationMinutes,
    List<SiteType> PreferredSiteTypes,
    List<TripDayDto> Days,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<string> PreferredSiteTypesDisplay,
    string StatusDisplay = "",
    string ToleranceDisplay = "");
