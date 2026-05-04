using Domain.Common.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.SponsorAggregate;

public class Sponsor : BaseAuditableEntity, IAggregateRoot
{   
    public SponsorType Type { get; private set; }
    public SponsorTier Tier { get; private set; }
    public SponsorStatus Status { get; private set; }
    public GeoLocation Location { get; private set; } = null!;

    public DateRange ContractDate { get; private set; } = null!;
    public string? MainImageUrl { get; set; }
    public string? WebsiteUrl { get; private set; }
    public PhoneNumber? ContactPhone { get; private set; }
    public Email? ContactEmail { get; private set; }

    public int TotalRedemptions { get; private set; }

    private readonly List<Offer> _offers = [];
    public IReadOnlyCollection<Offer> Offers => _offers.AsReadOnly();

    private readonly List<SponsorImage> _images = [];
    public IReadOnlyCollection<SponsorImage> Images => _images.AsReadOnly();

    private readonly List<SponsorLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<SponsorLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();

    private Sponsor() { }
    private Sponsor(
        SponsorType type,
        SponsorTier tier,
        GeoLocation location,
        DateRange dateRange,
        string? websiteUrl,
        PhoneNumber? contactPhone,
        Email? contactEmail)
    {
        Type = type;
        Tier = tier;
        Location = location;
        ContractDate = dateRange;
        WebsiteUrl = websiteUrl;
        ContactPhone = contactPhone;
        ContactEmail = contactEmail;

        Status = SponsorStatus.Inactive;
        TotalRedemptions = 0;
    }

    public static Result<Sponsor> Create(
        string title,
        string description,
        Address address,
        SponsorType type,
        SponsorTier tier,
        GeoLocation location,
        DateRange dateRange,
        string? websiteUrl,
        PhoneNumber? contactPhone,
        Email? contactEmail)
    {
        var sponsor = new Sponsor(
            type,
            tier,
            location,
            dateRange,
            websiteUrl?.Trim(),
            contactPhone,
            contactEmail);
        
        sponsor.AddLocalizedContent(LanguageCode.English, title, description, address);

        return sponsor;
    }

    public Result UpdateBasicInfo(SponsorType type)
    {
        Type = type;

        return Result.Success();
    }

    public void UpdateLocation(GeoLocation location)
    {
        if (location != Location)
        {
            Location = location;
        }
    }

    public void SetContactInfo(PhoneNumber? contactPhone, Email? contactEmail, string? website)
    {
        if (ContactPhone != contactPhone || contactEmail != ContactEmail || !string.IsNullOrWhiteSpace(website))
        {
            ContactPhone = contactPhone;
            ContactEmail = contactEmail;
            WebsiteUrl = website;
        }
    }

    public void UpdateTier(SponsorTier tier)
    {
        if (Tier == tier) return;

        Tier = tier;
        // RaiseDomainEvent(new SponsorTierChangedEvent(Id, tier));
    }

    public Result ExtendContract(DateTime newEndDate)
    {
        if (newEndDate <= ContractDate.EndDate)
            return SponsorErrors.InvalidExtendDate;

        var result = DateRange.Create(ContractDate.StartDate, newEndDate);
        if (result.Failed)
            return result;

        ContractDate = result.Value;

        return Result.Success();
    }

    public Result Activate()
    {
        if (DateTime.UtcNow > ContractDate.EndDate)
            return SponsorErrors.ExpiredContract;

        Status = SponsorStatus.Active;

        return Result.Success();
    }

    public void Deactivate()
    {
        Status = SponsorStatus.Inactive;
    }

    public Result<Offer> AddOffer(
        Money? discount,
        int? discountPercentage,
        DateRange validityPeriod,
        int? maxRedemptions,
        string? promoCode)
    {
        var offerResult = Offer.Create(
            discount,
            discountPercentage,
            validityPeriod,
            maxRedemptions,
            promoCode);

        if (offerResult.Failed)
            return offerResult;

        _offers.Add(offerResult.Value);

        return Result.Success(offerResult.Value);
    }

    public Result RemoveOffer(Guid offerId)
    {
        var offer = _offers.FirstOrDefault(o => o.Id == offerId);
        if (offer == null)
            return SponsorErrors.OfferNotFound(offerId);

        _offers.Remove(offer);

        return Result.Success();
    }

    public Result<Offer> UpdateOffer(
        Guid offerId,
        Money? discount,
        int? discountPercentage,
        DateRange validityPeriod)
    {
        var offer = _offers.FirstOrDefault(o => o.Id == offerId);
        if (offer == null)
            return SponsorErrors.OfferNotFound(offerId);

        var result = offer.Update(discount, discountPercentage);
        if (result.Failed)
            return result;

        result = offer.ExtendValidity(validityPeriod.EndDate);
        if (result.Failed)
            return result;

        return Result.Success(offer);
    }

    public Result<SponsorImage> AddImage(StorageKey storageKey, string imageUrl, bool isMain, int displayOrder, string? caption = null)
    {
        var imageResult = SponsorImage.Create(storageKey, imageUrl, isMain, displayOrder, caption);
        if (imageResult.Failed)
            return imageResult;

        if (isMain)
        {
            foreach (var img in _images)
                img.SetAsMain(false);

            SetMainImage(imageUrl);
        } else if (MainImageUrl == null)
        {
            SetMainImage(imageUrl);
        }

        _images.Add(imageResult.Value);

        return Result.Success(imageResult.Value);
    }

    public Result RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            return SponsorErrors.ImageNotFound;

        _images.Remove(image);
        if (image.IsMain && _images.Count > 0)
        {
            var newMain = _images.First();
            newMain.SetAsMain(true);
            SetMainImage(newMain.ImageUrl);
        }
        else if (image.IsMain)
        {
            MainImageUrl = null;
        }

        return Result.Success();
    }

    public Result RedeemOffer(Guid offerId)
    {
        var offer = _offers.FirstOrDefault(o => o.Id == offerId);
        if (offer == null)
            return SponsorErrors.OfferNotFound(offerId);

        var offerResult = offer.Redeem();
        if (offerResult.Failed)
            return offerResult;

        TotalRedemptions++;

        return Result.Success();
    }

    public Result<SponsorLocalizedContent> AddLocalizedContent(LanguageCode language, string title, string description, Address address)
    {
        var contentResult = SponsorLocalizedContent.Create(language, title, description, address);
        if (contentResult.Failed)
            return contentResult;

        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        _localizedContents.Add(contentResult.Value);

        return Result.Success(contentResult.Value);
    }

    public Result UpdateLocalizedContent(LanguageCode language, string title, string description, Address address)
    {
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing == null)
            return SponsorErrors.LocalizedContentNotFound;

        Result result = existing.Update(title, description);
        if (result.Failed)
            return result;
        
        existing.UpdateAddress(address);

        return Result.Success();
    }


    public IEnumerable<Offer> GetActiveOffers()
    {
        return _offers.Where(o => o.IsActive && o.IsValid());
    }

    private void SetMainImage(string imageUrl)
    {
        if (MainImageUrl != imageUrl)
        {
            MainImageUrl = imageUrl;
        }
    }
}
