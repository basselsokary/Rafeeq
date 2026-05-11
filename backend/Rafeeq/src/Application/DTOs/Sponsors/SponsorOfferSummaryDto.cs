using Application.DTOs.Common;

namespace Application.DTOs.Sponsors;

public record SponsorOfferSummaryDto(
    Guid Id,
    Guid SponsorId,
    string SponsorTitle,
    string Title,
    string Description,
    MoneyDto? DiscountAmount,
    int? DiscountPercentage,
    int DaysLeft);
