using Domain.Common;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.SponsorAggregate;

public class SponsorOffer : BaseAuditableEntity
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Money? DiscountAmount { get; private set; }
    public int? DiscountPercentage { get; private set; }
    public DateRange ValidityPeriod { get; private set; } = null!;
    public string? TermsAndConditions { get; private set; }
    public bool IsActive { get; private set; }
    public int RedemptionCount { get; private set; }
    public int? MaxRedemptions { get; private set; }
    public string? PromoCode { get; private set; }

    private SponsorOffer() { }
    private SponsorOffer(
        string title,
        string description,
        Money? discountAmount,
        int? discountPercentage,
        DateRange validityPeriod,
        string? termsAndConditions)
    {
        Title = title;
        Description = description;
        DiscountAmount = discountAmount;
        DiscountPercentage = discountPercentage;
        ValidityPeriod = validityPeriod;
        TermsAndConditions = termsAndConditions;
        IsActive = true;
        RedemptionCount = 0;
    }

    internal static SponsorOffer Create(
        string title,
        string description,
        Money? discountAmount,
        int? discountPercentage,
        DateRange validityPeriod,
        string? termsAndConditions = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessRuleValidationException("Offer title cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Offer description cannot be empty.");

        if (discountAmount == null && discountPercentage == null)
            throw new BusinessRuleValidationException("Either discount amount or percentage must be provided.");

        if (discountPercentage.HasValue && (discountPercentage.Value < 0 || discountPercentage.Value > 100))
            throw new BusinessRuleValidationException("Discount percentage must be between 0 and 100.");

        return new SponsorOffer(
            title.Trim(),
            description.Trim(),
            discountAmount,
            discountPercentage,
            validityPeriod,
            termsAndConditions?.Trim());
    }

    internal void Update(
        string title,
        string description,
        Money? discountAmount,
        int? discountPercentage,
        string? termsAndConditions)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessRuleValidationException("Offer title cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Offer description cannot be empty.");

        if (discountAmount == null && discountPercentage == null)
            throw new BusinessRuleValidationException("Either discount amount or percentage must be provided.");

        if (discountPercentage.HasValue && (discountPercentage.Value < 0 || discountPercentage.Value > 100))
            throw new BusinessRuleValidationException("Discount percentage must be between 0 and 100.");

        Title = title.Trim();
        Description = description.Trim();
        DiscountAmount = discountAmount;
        DiscountPercentage = discountPercentage;
        TermsAndConditions = termsAndConditions?.Trim();
        MarkAsUpdated();
    }

    internal void IncrementRedemption()
    {
        if (!IsActive)
            throw new InvalidOperationDomainException("Cannot redeem an inactive offer.");

        if (!IsValid())
            throw new InvalidOperationDomainException("Cannot redeem an expired offer.");

        if (MaxRedemptions.HasValue && RedemptionCount >= MaxRedemptions.Value)
            throw new InvalidOperationDomainException("Maximum redemptions reached.");

        RedemptionCount++;
        MarkAsUpdated();
    }

    public void SetMaxRedemptions(int maxRedemptions)
    {
        if (maxRedemptions < 0)
            throw new BusinessRuleValidationException("Max redemptions cannot be negative.");

        MaxRedemptions = maxRedemptions;
        MarkAsUpdated();
    }

    public void SetPromoCode(string promoCode)
    {
        if (string.IsNullOrWhiteSpace(promoCode))
            throw new BusinessRuleValidationException("Promo code cannot be empty.");

        PromoCode = promoCode.Trim().ToUpperInvariant();
        MarkAsUpdated();
    }

    public void Activate()
    {
        if (!IsValid())
            throw new InvalidOperationDomainException("Cannot activate an expired offer.");

        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public bool IsValid()
    {
        var now = DateTime.UtcNow;
        return ValidityPeriod.IsWithinRange(now);
    }

    public bool CanBeRedeemed()
    {
        return IsActive && IsValid() && (!MaxRedemptions.HasValue || RedemptionCount < MaxRedemptions.Value);
    }
}
