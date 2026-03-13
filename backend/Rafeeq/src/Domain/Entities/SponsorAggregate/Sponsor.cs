using Domain.Common.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Shared.Models;

namespace Domain.Entities.SponsorAggregate;

public class Sponsor : BaseAuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public SponsorType Type { get; private set; }
    public SponsorTier Tier { get; private set; }
    public GeoLocation Location { get; private set; } = null!;
    
    public Address Address { get; private set; } = null!;
    public string? Website { get; private set; }
    public PhoneNumber? ContactPhone { get; private set; }
    public Email? ContactEmail { get; private set; }
    public DateTime ContractStartDate { get; private set; }
    public DateTime ContractEndDate { get; private set; }
    
    public int TotalRedemptions { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Offer> _offers = [];
    public IReadOnlyCollection<Offer> Offers => _offers.AsReadOnly();
    
    private readonly List<SponsorImage> _images = [];
    public IReadOnlyCollection<SponsorImage> Images => _images.AsReadOnly();

    private Sponsor() { }
    private Sponsor(
        string title,
        string description,
        SponsorType type,
        SponsorTier tier,
        GeoLocation location,
        Address address,
        DateTime contractStartDate,
        DateTime contractEndDate)
    {
        Title = title;
        Description = description;
        Type = type;
        Tier = tier;
        Location = location;
        Address = address;
        ContractStartDate = contractStartDate;
        ContractEndDate = contractEndDate;
        
        IsActive = false;
        TotalRedemptions = 0;
    }

    public static Result<Sponsor> Create(
        string title,
        string description,
        SponsorType type,
        SponsorTier tier,
        GeoLocation location,
        Address address,
        DateTime contractStartDate,
        DateTime contractEndDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            return SponsorErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(description))
            return SponsorErrors.DescriptionRequired;

        if (contractStartDate >= contractEndDate)
            return SponsorErrors.InvalidDate;

        var sponsor = new Sponsor(
            title.Trim(),
            description.Trim(),
            type,
            tier,
            location,
            address,
            contractStartDate,
            contractEndDate);

        // sponsor.RaiseDomainEvent(new SponsorCreatedEvent(sponsor.Id, sponsor.Title, sponsor.Type));

        return sponsor;
    }

    public Result UpdateBasicInfo(string title, string description, SponsorType type)
    {
        if (string.IsNullOrWhiteSpace(title))
            return SponsorErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(description))
            return SponsorErrors.DescriptionRequired;

        Title = title.Trim();
        Description = description.Trim();
        Type = type;
        MarkAsUpdated();

        return Result.Success();
    }

    public void UpdateLocation(GeoLocation location, Address address)
    {
        if (location != Location || address != Address)
        {
            Location = location;
            Address = address;
            MarkAsUpdated();
        }
    }

    public void SetContactInfo(PhoneNumber contactPhone, Email contactEmail, string? website)
    {
        if (ContactPhone != contactPhone || contactEmail != ContactEmail || !string.IsNullOrWhiteSpace(website))
        {
            ContactPhone = contactPhone;
            ContactEmail = contactEmail;
            Website = website;
            MarkAsUpdated();
        }
    }

    public void UpdateTier(SponsorTier tier)
    {
        if (Tier == tier) return;

        Tier = tier;
        MarkAsUpdated();
        // RaiseDomainEvent(new SponsorTierChangedEvent(Id, tier));
    }

    public Result ExtendContract(DateTime newEndDate)
    {
        if (newEndDate <= ContractEndDate)
            return SponsorErrors.InvalidExtendDate;

        ContractEndDate = newEndDate;
        MarkAsUpdated();

        return Result.Success();
    }

    public Result Activate()
    {
        if (DateTime.UtcNow > ContractEndDate)
            return SponsorErrors.ExpiredContract;

        IsActive = true;
        MarkAsUpdated();

        return Result.Success();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public Result AddOffer(
        string title,
        string description,
        Money? discount,
        int? discountPercentage,
        DateRange validityPeriod,
        string? termsAndConditions,
        int? maxRedemptions)
    {
        var offerResult = Offer.Create(
            title,
            description,
            discount,
            discountPercentage,
            validityPeriod,
            termsAndConditions,
            maxRedemptions);
        
        if (offerResult.Failed)
            return offerResult;

        _offers.Add(offerResult.Value);
        MarkAsUpdated();

        return Result.Success();
    }

    public Result RemoveOffer(Guid offerId)
    {
        var offer = _offers.FirstOrDefault(o => o.Id == offerId);
        if (offer == null)
            return SponsorErrors.OfferNotFound(offerId);


        _offers.Remove(offer);
        MarkAsUpdated();

        return Result.Success();
    }

    public Result UpdateOffer(
        Guid offerId,
        string title,
        string description,
        Money? discount,
        int? discountPercentage,
        string? termsAndConditions)
    {
        var offer = _offers.FirstOrDefault(o => o.Id == offerId);
        if (offer == null)
            return SponsorErrors.OfferNotFound(offerId);

        var offerResult = offer.Update(title, description, discount, discountPercentage, termsAndConditions);
        if (offerResult.Failed)
            return offerResult;

        MarkAsUpdated();

        return Result.Success();
    }

    public Result AddImage(string imageUrl, bool isMain, string? caption = null)
    {
        var imageResult = SponsorImage.Create(imageUrl, isMain, caption);
        if (imageResult.Failed)
            return imageResult;
        
        if (isMain)
        {
            foreach (var img in _images)
                img.SetAsMain(false);
        }

        _images.Add(imageResult.Value);
        MarkAsUpdated();

        return Result.Success();
    }

    public Result RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            return SponsorErrors.ImageNotFound;

        _images.Remove(image);
        MarkAsUpdated();

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
        
        MarkAsUpdated();

        return Result.Success();
    }

    public bool IsContractValid()
    {
        var now = DateTime.UtcNow;
        return now >= ContractStartDate && now <= ContractEndDate;
    }

    public IEnumerable<Offer> GetActiveOffers()
    {
        return _offers.Where(o => o.IsActive && o.IsValid());
    }
}
