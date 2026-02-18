using Domain.Common.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.Common.Exceptions;
using static Domain.Common.Constants.DomainConstants.Review;

namespace Domain.Entities.SponsorAggregate;

/// <summary>
/// Represents a sponsor (restaurant, hotel, shop, etc.) that can offer promotions to tourists
/// </summary>
public class Sponsor : BaseAuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public SponsorType Type { get; private set; }
    public SponsorTier Tier { get; private set; }
    public GeoLocation Location { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public string? Website { get; private set; }
    public PhoneNumber ContactPhone { get; private set; } = null!;
    public Email ContactEmail { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime ContractStartDate { get; private set; }
    public DateTime ContractEndDate { get; private set; }
    public double AverageRating { get; private set; }
    public int TotalRedemptions { get; private set; }

    private readonly List<SponsorOffer> _offers = [];
    public IReadOnlyCollection<SponsorOffer> Offers => _offers.AsReadOnly();
    
    private readonly List<SponsorImage> _images = [];
    public IReadOnlyCollection<SponsorImage> Images => _images.AsReadOnly();

    private Sponsor() { }

    private Sponsor(
        string name,
        string description,
        SponsorType type,
        SponsorTier tier,
        GeoLocation location,
        Address address,
        PhoneNumber phoneNumber,
        Email email,
        DateTime contractStartDate,
        DateTime contractEndDate)
    {
        Name = name;
        Description = description;
        Type = type;
        Tier = tier;
        Location = location;
        Address = address;
        ContractStartDate = contractStartDate;
        ContractEndDate = contractEndDate;
        ContactPhone = phoneNumber;
        ContactEmail = email;
        
        IsActive = true;
        AverageRating = 0;
        TotalRedemptions = 0;
    }

    public static Sponsor Create(
        string name,
        string description,
        SponsorType type,
        SponsorTier tier,
        GeoLocation location,
        Address address,
        PhoneNumber phoneNumber,
        Email email,
        DateTime contractStartDate,
        DateTime contractEndDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Sponsor name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Sponsor description cannot be empty.");

        if (contractStartDate >= contractEndDate)
            throw new BusinessRuleValidationException("Contract start date must be before end date.");

        var sponsor = new Sponsor(
            name.Trim(),
            description.Trim(),
            type,
            tier,
            location,
            address,
            phoneNumber,
            email,
            contractStartDate,
            contractEndDate);

        // sponsor.RaiseDomainEvent(new SponsorCreatedEvent(sponsor.Id, sponsor.Name, sponsor.Type));

        return sponsor;
    }

    public void UpdateBasicInfo(string name, string description, SponsorType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Sponsor name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Sponsor description cannot be empty.");

        Name = name.Trim();
        Description = description.Trim();
        Type = type;
        MarkAsUpdated();
    }

    public void UpdateLocation(GeoLocation location, Address address)
    {
        Location = location;
        Address = address;
        MarkAsUpdated();
    }

    public void UpdateContactInfo(PhoneNumber contactPhone, Email contactEmail, string? website)
    {
        ContactPhone = contactPhone;
        ContactEmail = contactEmail;
        Website = website;
        MarkAsUpdated();
    }

    public void UpdateTier(SponsorTier tier)
    {
        if (Tier == tier) return;

        Tier = tier;
        MarkAsUpdated();
        // RaiseDomainEvent(new SponsorTierChangedEvent(Id, tier));
    }

    public void ExtendContract(DateTime newEndDate)
    {
        if (newEndDate <= ContractEndDate)
            throw new BusinessRuleValidationException("New end date must be after current end date.");

        ContractEndDate = newEndDate;
        MarkAsUpdated();
    }

    public void Activate()
    {
        if (DateTime.UtcNow > ContractEndDate)
            throw new InvalidOperationDomainException("Cannot activate sponsor with expired contract.");

        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public void AddOffer(
        string title,
        string description,
        Money? discount,
        int? discountPercentage,
        DateRange validityPeriod,
        string? termsAndConditions = null)
    {
        var offer = SponsorOffer.Create(
            title,
            description,
            discount,
            discountPercentage,
            validityPeriod,
            termsAndConditions);

        _offers.Add(offer);
        MarkAsUpdated();
    }

    public void RemoveOffer(Guid offerId)
    {
        var offer = _offers.FirstOrDefault(o => o.Id == offerId)
            ?? throw new EntityNotFoundException(nameof(SponsorOffer), offerId);

        _offers.Remove(offer);
        MarkAsUpdated();
    }

    public void UpdateOffer(
        Guid offerId,
        string title,
        string description,
        Money? discount,
        int? discountPercentage,
        string? termsAndConditions)
    {
        var offer = _offers.FirstOrDefault(o => o.Id == offerId)
            ?? throw new EntityNotFoundException(nameof(SponsorOffer), offerId);

        offer.Update(title, description, discount, discountPercentage, termsAndConditions);
        MarkAsUpdated();
    }

    public void AddImage(string imageUrl, bool isMain, string? caption = null)
    {
        var image = SponsorImage.Create(imageUrl, isMain, caption);
        
        if (isMain)
        {
            foreach (var img in _images)
                img.SetAsMain(false);
        }

        _images.Add(image);
        MarkAsUpdated();
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new EntityNotFoundException(nameof(SponsorImage), imageId);

        _images.Remove(image);
        MarkAsUpdated();
    }

    public void UpdateImage(Guid imageId, bool isMain, string? caption = null)
    {
        var image = _images.FirstOrDefault(img => img.Id == imageId)
            ?? throw new EntityNotFoundException(nameof(SponsorImage), imageId);
        
        if (isMain)
        {
            foreach (var img in _images)
                img.SetAsMain(false);
            
            image.SetAsMain(isMain);
        }

        image.UpdateCaption(caption);
        MarkAsUpdated();
    }

    public void IncrementRedemptionCount(SponsorOffer offer)
    {
        TotalRedemptions++;
        offer.IncrementRedemption();
        MarkAsUpdated();
    }

    public void UpdateRating(double averageRating, int totalReviews)
    {
        if (averageRating < 0 || averageRating > MaxRatingValue)
            throw new BusinessRuleValidationException($"Average rating must be between 0 and {MaxRatingValue}.");

        if (totalReviews < 0)
            throw new BusinessRuleValidationException("Total reviews cannot be negative.");

        AverageRating = averageRating;
        MarkAsUpdated();
    }

    public bool IsContractValid()
    {
        var now = DateTime.UtcNow;
        return now >= ContractStartDate && now <= ContractEndDate;
    }

    public IEnumerable<SponsorOffer> GetActiveOffers()
    {
        return _offers.Where(o => o.IsActive && o.IsValid());
    }
}
