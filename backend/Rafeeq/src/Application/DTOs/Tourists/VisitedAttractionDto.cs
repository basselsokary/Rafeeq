using Domain.Enums;

namespace Application.DTOs.Tourists;

public record VisitedSiteDto(
    Guid Id,
    Guid SiteId,
    string SiteName,
    SiteType SiteType,
    string? SiteImageUrl,
    double SiteAverageRating,
    DateTime VisitDate,
    int DurationMinutes,
    int Rating,
    bool HasRating,
    string? Notes,
    string SiteTypeDisplay = "");
