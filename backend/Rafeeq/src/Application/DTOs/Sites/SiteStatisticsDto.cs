namespace Application.DTOs.Sites;

public record SiteStatisticsDto(
    Guid SiteId,
    string SiteName,
    int TotalVisits,
    int TotalReviews,
    double AverageRating,
    RatingDistributionDto RatingDistribution,
    int TotalFavorites,
    int VisitsThisMonth,
    int VisitsThisYear,
    double AverageVisitDurationMinutes);
