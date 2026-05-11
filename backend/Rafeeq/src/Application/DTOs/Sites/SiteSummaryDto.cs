using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Sites;

public record SiteSummaryDto(
    Guid Id,
    string Name,
    string Description,
    string City,
    SiteType Type,
    string? PrimaryImageUrl,
    double AverageRating,
    int TotalRating,
    LocationDto Location,
    bool IsFree,
    string? Badge,
    string TypeDisplay = "");
