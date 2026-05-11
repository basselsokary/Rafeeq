using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Admins;

public record AdminSiteDetailDto(
    Guid Id,
    string CityName,
    string Name,
    string Description,
    SiteType Type,
    SiteStatus Status,
    LocationDto Location,
    string Address,
    string? ContactPhone,
    string? Website,
    string? MainImageUrl,
    double AverageRating,
    int TotalRatings,
    TicketDto? EntryTicket,
    bool IsFree,
    bool IsFeatured,
    AuditInfoDto AuditInfo);

public record AdminSiteLocalizedContentDto(
    Guid Id,
    LanguageCode Language,
    string Name,
    string Description,
    string Address,
    string? EntryFeeNotes,
    AuditInfoDto AuditInfo);

public record AdminSiteFacilityDto(
    FacilityType Type,
    string Name);

public record AdminSiteNearestTransportationDto(
    Guid Id,
    TransportationType Type,
    LocationDto Location,
    double DistanceKm,
    bool IsOperational,
    bool HasAccessibility,
    TimeRangeDto? OperatingHours,
    List<AdminSiteNearestTransportationLocalizedContentDto> LocalizedContent,
    AuditInfoDto AuditInfo);

public record AdminSiteNearestTransportationLocalizedContentDto(
    LanguageCode Language,
    string Name,
    string? Description,
    string? Address,
    AuditInfoDto AuditInfo);

public sealed record AdminSiteOpeningHourDto(
    WeekDay Day,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool IsClosed);

public sealed record AdminSiteDashboardDto(
    int TotalSites,
    int ActiveSites,
    int FeaturedSites,
    int HiddenGemSites,
    double AverageRating,
    int TotalRating);