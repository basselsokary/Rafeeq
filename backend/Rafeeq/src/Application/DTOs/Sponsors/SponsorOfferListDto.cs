using Application.DTOs.Common;

namespace Application.DTOs.Sponsors;

public record SponsorOfferListDto(
    Guid Id,
    Guid SponsorId,
    string SponsorName,
    string Title,
    string Description,
    MoneyDto? DiscountAmount,
    int? DiscountPercentage,
    DateRangeDto ValidityPeriod,
    bool IsValid,
    int DaysUntilExpiry,
    int RedemptionCount,
    bool CanBeRedeemed,
    DateTime CreatedAt);
