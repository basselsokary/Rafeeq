namespace Application.DTOs.Sponsors;

public record SponsorStatisticsDto(
    Guid SponsorId,
    string Name,
    int TotalClicks,
    int TotalRedemptions,
    int ActiveOffers,
    int TotalOffers,
    double AverageRating,
    int TotalReviews,
    int ClicksThisMonth,
    int RedemptionsThisMonth,
    double ConversionRate); // Redemptions / Clicks
