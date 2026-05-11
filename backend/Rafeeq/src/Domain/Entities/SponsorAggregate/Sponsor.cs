using Domain.Common.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.SponsorAggregate;

public partial class Sponsor : BaseAuditableEntity, IAggregateRoot
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

        sponsor.RaiseDomainEvent(new SponsorCreatedEvent(sponsor.Id, title.Trim(), type));

        return sponsor;
    }

    public Result UpdateBasicInfo(SponsorType type)
    {
        Type = type;

        RaiseDomainEvent(new SponsorUpdatedEvent(Id));

        return Result.Success();
    }

    public void UpdateLocation(GeoLocation location)
    {
        if (location != Location)
        {
            Location = location;
            RaiseDomainEvent(new SponsorUpdatedEvent(Id));
        }
    }

    public void SetContactInfo(PhoneNumber? contactPhone, Email? contactEmail, string? website)
    {
        if (ContactPhone != contactPhone || contactEmail != ContactEmail || !string.IsNullOrWhiteSpace(website))
        {
            ContactPhone = contactPhone;
            ContactEmail = contactEmail;
            WebsiteUrl = website;

            RaiseDomainEvent(new SponsorUpdatedEvent(Id));
        }
    }

    public void UpdateTier(SponsorTier tier)
    {
        if (Tier == tier) return;

        Tier = tier;
        RaiseDomainEvent(new SponsorTierChangedEvent(Id, tier));
    }

    public Result ExtendContract(DateTime newEndDate)
    {
        if (newEndDate <= ContractDate.EndDate)
            return SponsorErrors.InvalidExtendDate;

        var result = DateRange.Create(ContractDate.StartDate, newEndDate);
        if (result.Failed)
            return result;

        ContractDate = result.Value;

        RaiseDomainEvent(new SponsorUpdatedEvent(Id));

        return Result.Success();
    }

    public Result Activate()
    {
        if (DateTime.UtcNow > ContractDate.EndDate)
            return SponsorErrors.ExpiredContract;

        Status = SponsorStatus.Active;

        RaiseDomainEvent(new SponsorUpdatedEvent(Id));

        return Result.Success();
    }

    public void Deactivate()
    {
        Status = SponsorStatus.Inactive;
        RaiseDomainEvent(new SponsorUpdatedEvent(Id));
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

        RaiseDomainEvent(new SponsorOfferChangedEvent(Id, offerResult.Value.Id));

        return Result.Success(offerResult.Value);
    }

    public Result RemoveOffer(Guid offerId)
    {
        var offer = _offers.FirstOrDefault(o => o.Id == offerId);
        if (offer == null)
            return SponsorErrors.OfferNotFound(offerId);

        _offers.Remove(offer);

        RaiseDomainEvent(new SponsorOfferChangedEvent(Id, offerId));

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

        RaiseDomainEvent(new SponsorOfferChangedEvent(Id, offerId));

        return Result.Success(offer);
    }

    public Result<SponsorImage> AddImage(Guid storedFileId, StorageKey storageKey, string imageUrl, bool isMain, int displayOrder, string? caption = null)
    {
        var imageResult = SponsorImage.Create(storedFileId, storageKey, imageUrl, isMain, displayOrder, caption);
        if (imageResult.Failed)
            return imageResult;

        if (isMain)
        {
            var mainImages = _images.Where(i => i.IsMain == true).ToList();
            foreach (var img in mainImages)
                img.SetAsMain(false);

            SetMainImage(imageUrl);
        } else if (MainImageUrl == null)
        {
            SetMainImage(imageUrl);
        }

        _images.Add(imageResult.Value);

        RaiseDomainEvent(new SponsorImageUpdatedEvent(Id));
        return Result.Success(imageResult.Value);
    }

    public Result<SponsorImage> RemoveImage(Guid imageId)
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

        RaiseDomainEvent(new SponsorImageUpdatedEvent(Id));
        return Result.Success(image);
    }

    public Result SetMainImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            return SponsorErrors.ImageNotFound;

        var mainImages = _images.Where(i => i.IsMain == true).ToList();
        foreach (var img in mainImages)
            img.SetAsMain(false);

        image.SetAsMain(true);
        SetMainImage(image.ImageUrl);

        RaiseDomainEvent(new SponsorImageUpdatedEvent(Id));
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

        RaiseDomainEvent(new SponsorOfferChangedEvent(Id, offerId));

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

        RaiseDomainEvent(new SponsorLocalizedContentUpdatedEvent(Id));

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

        RaiseDomainEvent(new SponsorLocalizedContentUpdatedEvent(Id));

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
