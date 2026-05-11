using Domain.Enums;

namespace Application.DTOs.Sponsors;

public record SponsorSummaryDto(
    Guid Id,
    string Name,
    SponsorType Type,
    SponsorTier Tier,
    string? PrimaryImageUrl,
    double AverageRating,
    int ActiveOffersCount,
    bool HasActiveOffers,
    string TypeDisplay = "",
    string TierDisplay = "");
