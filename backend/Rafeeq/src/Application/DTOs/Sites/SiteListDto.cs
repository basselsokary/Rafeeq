using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Sites;

public record SiteListDto(
    Guid Id,
    string CityName,
    string Name,
    string Description,
    SiteType Type,
    SiteStatus Status,
    LocationDto Location,
    string? PrimaryImageUrl,
    double AverageRating,
    int TotalRating,
    bool IsFree,
    bool IsFeatured,
    string TypeDisplay = "",
    string StatusDisplay = "");

public record SiteLookupDto(
    Guid Id,
    string Name);