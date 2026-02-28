using Domain.Common;
using Domain.ValueObjects;
using Shared.Models;

namespace Domain.Entities.SponsorAggregate;

public class Offer : BaseAuditableEntity
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

    private Offer() { }
    private Offer(
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
        
        IsActive = false;
        RedemptionCount = 0;
    }

    internal static Result<Offer> Create(
        string title,
        string description,
        Money? discountAmount,
        int? discountPercentage,
        DateRange validityPeriod,
        string? termsAndConditions = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return SponsorErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(description))
            return SponsorErrors.DescriptionRequired;
            
        if (discountAmount == null && discountPercentage == null)
            return SponsorErrors.DiscountRequired;

        if (discountPercentage.HasValue && (discountPercentage.Value < 0 || discountPercentage.Value > 100))
            return SponsorErrors.DiscountPercentageInvalid;
            
        return new Offer(
            title.Trim(),
            description.Trim(),
            discountAmount,
            discountPercentage,
            validityPeriod,
            termsAndConditions?.Trim());
    }

    internal Result Update(
        string title,
        string description,
        Money? discountAmount,
        int? discountPercentage,
        string? termsAndConditions)
    {
        if (string.IsNullOrWhiteSpace(title))
            return SponsorErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(description))
            return SponsorErrors.DescriptionRequired;
            
        if (discountAmount == null && discountPercentage == null)
            return SponsorErrors.DiscountRequired;

        if (discountPercentage.HasValue && (discountPercentage.Value < 0 || discountPercentage.Value > 100))
            return SponsorErrors.DiscountPercentageInvalid;
        
        Title = title.Trim();
        Description = description.Trim();
        DiscountAmount = discountAmount;
        DiscountPercentage = discountPercentage;
        TermsAndConditions = termsAndConditions?.Trim();
        MarkAsUpdated();

        return Result.Success();
    }

    internal Result IncrementRedemption()
    {
        if (!IsActive)
            return SponsorErrors.InactiveOffer;

        if (!IsValid())
            return SponsorErrors.ExpiredOffer;

        if (MaxRedemptions.HasValue && RedemptionCount >= MaxRedemptions.Value)
            return SponsorErrors.MaximumRedemptionsReached;

        RedemptionCount++;
        MarkAsUpdated();

        return Result.Success();
    }

    public Result SetMaxRedemptions(int maxRedemptions)
    {
        if (maxRedemptions < 0)
            return SponsorErrors.NegativeRedemptionsNumber;

        MaxRedemptions = maxRedemptions;
        MarkAsUpdated();

        return Result.Success();
    }

    public Result SetPromoCode(string promoCode)
    {
        if (string.IsNullOrWhiteSpace(promoCode))
            return SponsorErrors.PromoCodeRequired;

        PromoCode = promoCode.Trim().ToUpperInvariant();
        MarkAsUpdated();

        return Result.Success();
    }

    public Result Activate()
    {
        if (!IsValid())
            return SponsorErrors.InactiveOffer;

        IsActive = true;
        MarkAsUpdated();

        return Result.Success();
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
