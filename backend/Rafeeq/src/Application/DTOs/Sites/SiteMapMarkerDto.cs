using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Sites;

public record SiteMapMarkerDto(
    Guid Id,
    string Name,
    SiteType Type,
    LocationDto Location,
    double AverageRating,
    string? PrimaryImageUrl,
    bool IsFeatured,
    string TypeDisplay = "");
