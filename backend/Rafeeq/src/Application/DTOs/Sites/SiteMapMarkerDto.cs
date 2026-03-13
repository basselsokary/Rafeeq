namespace Application.DTOs.Sites;

/// <summary>
/// DTO for map markers
/// </summary>
public record SiteMapMarkerDto(
    Guid Id,
    string Name,
    string Type,
    double Latitude,
    double Longitude,
    double AverageRating,
    string? PrimaryImageUrl,
    bool IsFeatured);
