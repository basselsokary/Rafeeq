using Domain.Common;
using Domain.ValueObjects;
using Domain.Enums;
using Shared;

namespace Domain.Entities.SponsorAggregate;

public class Offer : BaseAuditableEntity
{
    public Sponsor Sponsor { get; set; } = null!;
    public Guid SponsorId { get; set; }
    
    public Money? DiscountAmount { get; private set; }
    public int? DiscountPercentage { get; private set; }

    public DateRange ValidityPeriod { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public int RedemptionCount { get; private set; }
    public int? MaxRedemptions { get; private set; }
    public string? PromoCode { get; private set; }

    private readonly List<OfferLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<OfferLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();

    private Offer() { }
    private Offer(
        Money? discountAmount,
        int? discountPercentage,
        DateRange validityPeriod,
        int? maxRedemptions,
        string? promoCode)
    {
        DiscountAmount = discountAmount;
        DiscountPercentage = discountPercentage;
        ValidityPeriod = validityPeriod;
        MaxRedemptions = maxRedemptions;
        PromoCode = promoCode;

        IsActive = false;
        RedemptionCount = 0;
    }

    internal static Result<Offer> Create(
        Money? discountAmount,
        int? discountPercentage,
        DateRange validityPeriod,
        int? maxRedemptions = null,
        string? promoCode = null)
    {
        if (discountAmount == null && discountPercentage == null)
            return SponsorErrors.DiscountRequired;

        if (discountPercentage.HasValue && (discountPercentage.Value < 0 || discountPercentage.Value > 100))
            return SponsorErrors.DiscountPercentageInvalid;
        
        var offer = new Offer(
            discountAmount,
            discountPercentage,
            validityPeriod,
            maxRedemptions,
            promoCode);

        return offer;
    }

    internal Result<Offer> Update(
        Money? discountAmount,
        int? discountPercentage)
    {
        if (discountAmount == null && discountPercentage == null)
            return SponsorErrors.DiscountRequired;

        if (discountPercentage.HasValue && (discountPercentage.Value < 0 || discountPercentage.Value > 100))
            return SponsorErrors.DiscountPercentageInvalid;

        DiscountAmount = discountAmount;
        DiscountPercentage = discountPercentage;

        return Result.Success(this);
    }

    public Result<Offer> ExtendValidity(DateTime newEndDate)
    {
        if (!IsActive)
            return SponsorErrors.InactiveOffer;

        if (newEndDate <= ValidityPeriod.EndDate)
            return SponsorErrors.NewEndDateMustBeLater;

        var result = DateRange.Create(ValidityPeriod.StartDate, newEndDate);
        if (result.Failed)
            return result.To<Offer>();

        ValidityPeriod = result.Value;

        return Result.Success(this);
    }

    internal Result Redeem()
    {
        if (!IsActive)
            return SponsorErrors.InactiveOffer;

        if (!IsValid())
            return SponsorErrors.ExpiredOffer;

        if (MaxRedemptions.HasValue && RedemptionCount >= MaxRedemptions.Value)
            return SponsorErrors.MaximumRedemptionsReached;

        RedemptionCount++;

        return Result.Success();
    }

    public Result SetMaxRedemptions(int maxRedemptions)
    {
        if (maxRedemptions < 0)
            return SponsorErrors.NegativeRedemptionsNumber;

        MaxRedemptions = maxRedemptions;

        return Result.Success();
    }

    public Result SetPromoCode(string promoCode)
    {
        if (string.IsNullOrWhiteSpace(promoCode))
            return SponsorErrors.PromoCodeRequired;

        PromoCode = promoCode.Trim().ToUpperInvariant();

        return Result.Success();
    }

    public Result Activate(DateRange? validityPeriod = null)
    {
        if (!IsValid(validityPeriod?.EndDate))
            return SponsorErrors.InactiveOffer;

        if (validityPeriod != null)
        {
            var result = DateRange.Create(validityPeriod.StartDate, validityPeriod.EndDate);
            if (result.Failed)
                return result;

            ValidityPeriod = result.Value;
        }

        IsActive = true;

        return Result.Success();
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public Result<OfferLocalizedContent> AddLocalizedContent(LanguageCode language, string title, string description, string? termsAndConditions)
    {
        var contentResult = OfferLocalizedContent.Create(language, title, description, termsAndConditions);
        if (contentResult.Failed)
            return contentResult.To<OfferLocalizedContent>();

        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        _localizedContents.Add(contentResult.Value);

        return Result.Success(contentResult.Value);
    }

    public Result<OfferLocalizedContent> UpdateLocalizedContent(Guid contentId, string title, string description, string? termsAndConditions)
    {
        var existing = _localizedContents.FirstOrDefault(lc => lc.Id == contentId);
        if (existing == null)
            return SponsorErrors.LocalizedContentNotFound;

        Result result = existing.Update(title, description, termsAndConditions);
        if (result.Failed)
            return result.To<OfferLocalizedContent>();

        return Result.Success(existing);
    }

    public bool IsValid(DateTime? referenceDate = null)
    {
        var now = referenceDate ?? DateTime.UtcNow;
        return ValidityPeriod.IsWithinRange(now);
    }

    public bool CanBeRedeemed()
    {
        return IsActive && IsValid() && (!MaxRedemptions.HasValue || RedemptionCount < MaxRedemptions.Value);
    }
}
