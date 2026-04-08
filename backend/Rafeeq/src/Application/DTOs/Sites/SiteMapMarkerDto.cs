using Application.DTOs.Common;

namespace Application.DTOs.Sites;

/// <summary>
/// DTO for map markers
/// </summary>
public record SiteMapMarkerDto(
    Guid Id,
    string Name,
    string Type,
    LocationDto Location,
    double AverageRating,
    string? PrimaryImageUrl,
    bool IsFeatured);
