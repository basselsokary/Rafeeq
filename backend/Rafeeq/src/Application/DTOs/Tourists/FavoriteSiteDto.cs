using Domain.Enums;

namespace Application.DTOs.Tourists;

public record FavoriteSiteDto(
    Guid Id,
    Guid SiteId,
    string SiteName,
    SiteType SiteType,
    string? SiteImageUrl,
    double AverageRating,
    string? Notes,
    DateTime AddedAt,
    string SiteTypeDisplay = "");
