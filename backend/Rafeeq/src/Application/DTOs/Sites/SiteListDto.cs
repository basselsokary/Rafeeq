using Application.DTOs.Common;

namespace Application.DTOs.Sites;

public record SiteListDto(
    Guid Id,
    string Name,
    string ShortDescription,
    string Type,
    string Status,
    LocationDto Location,
    string City,
    string? PrimaryImageUrl,
    double AverageRating,
    int TotalReviews,
    decimal? EntryFeeAmount,
    bool IsFree,
    bool IsFeatured);
