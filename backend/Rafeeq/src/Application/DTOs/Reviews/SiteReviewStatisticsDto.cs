namespace Application.DTOs.Reviews;

/// <summary>
/// Review statistics for an site
/// </summary>
public record SiteReviewStatisticsDto(
    Guid SiteId,
    string SiteName,
    double AverageRating,
    int TotalReviews,
    RatingDistributionDto RatingDistribution,
    int ReviewsThisMonth,
    int ReviewsThisYear,
    List<string> MostCommonKeywords);
