namespace Application.DTOs.Sponsors;

/// <summary>
/// Sponsor summary for cards
/// </summary>
public record SponsorSummaryDto(
    Guid Id,
    string Name,
    string Type,
    string Tier,
    string? PrimaryImageUrl,
    double AverageRating,
    int ActiveOffersCount,
    bool HasActiveOffers);
