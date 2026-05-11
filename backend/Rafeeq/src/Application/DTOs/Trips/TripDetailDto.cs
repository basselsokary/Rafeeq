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

public sealed record TripDayDto(
    Guid Id,
    int DayNumber,
    DateTime Date,
    MoneyDto? EstimatedDayCost,
    int TotalSites,
    int TotalDurationMinutes,
    string? Notes,
    List<TripSiteDto> Sites);

public sealed record TripSiteDto(
    Guid Id,
    string SiteName,
    string SiteImageUrl,
    SiteType SiteType,
    string CityName,
    LocationDto SiteLocation,
    MoneyDto EstimatedCost,
    int VisitOrder,
    TimeOnly PlannedArrivalTime,
    DateTime? ActualVisitTime,
    int EstimatedDurationMinutes,
    bool IsVisited,
    string SiteTypeDisplay = "");