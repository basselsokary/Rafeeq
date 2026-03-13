namespace Application.DTOs.Sites;

/// <summary>
/// Lightweight DTO for site list views (mobile list, search results)
/// </summary>
public record SiteListDto(
    Guid Id,
    string Name,
    string ShortDescription,
    string Type,
    string Status,
    double Latitude,
    double Longitude,
    string City,
    string? PrimaryImageUrl,
    double AverageRating,
    int TotalReviews,
    decimal? EntryFeeAmount,
    string? EntryFeeCurrency,
    bool IsFree,
    int EstimatedDurationMinutes,
    string Accessibility,
    bool IsFeatured,
    bool IsPopular,
    double? DistanceKm,
    bool? IsCurrentlyOpen);
