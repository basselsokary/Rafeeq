namespace Application.DTOs.Sponsors;

/// <summary>
/// Sponsor statistics DTO
/// </summary>
public record SponsorStatisticsDto(
    Guid SponsorId,
    string SponsorName,
    int TotalClicks,
    int TotalRedemptions,
    int ActiveOffers,
    int TotalOffers,
    double AverageRating,
    int TotalReviews,
    int ClicksThisMonth,
    int RedemptionsThisMonth,
    double ConversionRate); // Redemptions / Clicks
