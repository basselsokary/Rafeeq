using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Trips;

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