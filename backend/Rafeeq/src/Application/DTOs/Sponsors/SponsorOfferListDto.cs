using Application.DTOs.Common;

namespace Application.DTOs.Sponsors;

public record SponsorOfferListDto(
    Guid Id,
    Guid SponsorId,
    string SponsorTitle,
    string Title,
    string Description,
    MoneyDto? DiscountAmount,
    int? DiscountPercentage,
    DateRangeDto ValidityPeriod,
    bool IsValid,
    bool CanBeRedeemed,
    bool IsActive,
    DateTime CreatedAt);
