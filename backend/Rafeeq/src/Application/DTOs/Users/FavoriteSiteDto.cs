namespace Application.DTOs.Users;

/// <summary>
/// Favorite site DTO
/// </summary>
public record FavoriteSiteDto(
    Guid Id,
    Guid SiteId,
    string SiteName,
    string SiteType,
    string? SiteImageUrl,
    string City,
    double AverageRating,
    string? Notes,
    DateTime AddedAt);
