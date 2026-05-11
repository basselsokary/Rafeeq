using Application.DTOs.Common;

namespace Application.DTOs.Sponsors;

public record SponsorOfferDto(
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
    string? TermsAndConditions,
    string? PromoCode,
    int? MaxRedemptions,
    int RedemptionCount,
    bool CanBeRedeemed,
    bool IsActive,
    DateTime CreatedAt);
