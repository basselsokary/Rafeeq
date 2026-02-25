using Domain.Common;
using Domain.Exceptions;
using Domain.Common.Interfaces;
using Domain.Enums;
using Domain.ValueObjects;
using Shared.Models;
using Domain.Entities.AttractionAggregate;

namespace Domain.Entities.SiteAggregate;

public class Site : BaseAuditableEntity, IAggregateRoot
{
    public Guid CityId { get; private set; }
    
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public SiteStatus Status { get; private set; }
    public SiteType Type { get; private set; }
    public Address Address { get; private set; } = null!;
    public GeoLocation Location { get; private set; } = null!;
    public Money? EntryFee { get; private set; }

    public string? WebsiteUrl { get; private set; }
    public string? MainImageUrl { get; private set; }
    public string? ContactPhone { get; private set; }
    public double AverageRating { get; private set; }
    public int TotalReviews { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Attraction> _attractions = [];
    public IReadOnlyCollection<Attraction> Attractions => _attractions.AsReadOnly();
    
    private readonly List<NearestTransportation> _nearestTransportations = [];
    public IReadOnlyCollection<NearestTransportation> NearestTransportations => _nearestTransportations.AsReadOnly();
    
    private readonly List<SiteImage> _images = [];
    public IReadOnlyCollection<SiteImage> Images => _images.AsReadOnly();
    
    private readonly List<SiteLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<SiteLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();
    
    private readonly List<SiteFacility> _facilities = [];
    public IReadOnlyCollection<SiteFacility> Facilities => _facilities.AsReadOnly();

    private readonly List<OpeningHour> _openingHours = [];
    public IReadOnlyCollection<OpeningHour> OpeningHours => _openingHours.AsReadOnly();
    
    private Site() { }
    private Site(
        Guid cityId,
        string name,
        string description,
        GeoLocation location,
        Address address,
        SiteType type)
    {
        CityId = cityId;
        Name = name;
        Description = description;
        Location = location;
        Address = address;
        Type = type;
        
        IsActive = false;
        AverageRating = 0.0;
        TotalReviews = 0;
    }

    public static Site Create(
        Guid cityId,
        string name,
        string description,
        GeoLocation location,
        Address address,
        SiteType type)
    {
        if (cityId == Guid.Empty)
            throw new BusinessRuleValidationException("City ID cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Site name cannot be null or empty.");
        
        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Site description cannot be null or empty.");

        return new(cityId, name, description, location, address, type);
    }

    public void UpdateBasicInfo(string name, string description, SiteType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Site name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Site description cannot be empty.");

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

    public void UpdateCity(Guid cityId)
    {
        CityId = cityId;
        MarkAsUpdated();
    }

    public Result AddOpeningHours(DayOfWeek dayOfWeek, TimeRange openingTime, bool isClosed)
    {
        if (_openingHours.Any(o => o.DayOfWeek == dayOfWeek))
        {
            return Result.Failure(
                Error.Failure("OPENING_HOURS", $"Opening hours for {dayOfWeek} already exist."));
        }

        var openingHours = OpeningHour.Create(dayOfWeek, openingTime, isClosed);
        _openingHours.Add(openingHours);
        
        return Result.Success();
    }

    public Result UpdateOpeningHours(DayOfWeek dayOfWeek, TimeRange openingTime, bool isClosed)
    {
        var existing = _openingHours.FirstOrDefault(oh => oh.DayOfWeek == dayOfWeek);
        if (existing == null)
        {
            return Result.Failure(
                Error.Failure("", $"Opening hours for {dayOfWeek} does not exist."));
        }

        var newOpeningHours = OpeningHour.Create(dayOfWeek, openingTime, isClosed);
        if (existing.Equals(newOpeningHours))
        {
            // No changes, so we can skip the update
            return Result.Success();
        }

        _openingHours.Remove(existing);
        _openingHours.Add(newOpeningHours);

        return Result.Success();
    }

    public void SetEntryFee(Money fee)
    {
        EntryFee = fee;
        MarkAsUpdated();
    }

    public void RemoveEntryFee()
    {
        EntryFee = null;
        MarkAsUpdated();
    }

    public void SetContactInfo(string phone, string? websiteUrl)
    {
        ContactPhone = phone;
        WebsiteUrl = websiteUrl;
        MarkAsUpdated();
    }

    public void UpdateStatus(SiteStatus status)
    {
        if (Status == status) return;

        Status = status;
        MarkAsUpdated();
    }

    public void AddImage(string imageUrl, bool isMain, string? caption = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new BusinessRuleValidationException("Image URL cannot be empty.");

        if (isMain)
        {
            foreach (var img in _images)
                img.SetAsMain(false);
        }

        var image = SiteImage.Create(imageUrl, isMain, caption);
        _images.Add(image);
        MarkAsUpdated();
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            throw new EntityNotFoundException(nameof(SiteImage), imageId);

        _images.Remove(image);
        MarkAsUpdated();
    }

    public void AddLocalizedContent(LanguageCode language, string name, string description)
    {
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        var content = SiteLocalizedContent.Create(language, name, description);
        _localizedContents.Add(content);
        MarkAsUpdated();
    }

    public void AddFacility(string name, FacilityType type, string? description = null)
    {
        var facility = SiteFacility.Create(name, type, description);
        _facilities.Add(facility);
        MarkAsUpdated();
    }

    public void RemoveFacility(Guid facilityId)
    {
        var facility = _facilities.FirstOrDefault(f => f.Id == facilityId);
        if (facility == null)
            throw new EntityNotFoundException(nameof(SiteFacility), facilityId);

        _facilities.Remove(facility);
        MarkAsUpdated();
    }

    public bool IsOpenAt(DayOfWeek day, TimeSpan time)
    {
        var hours = _openingHours.FirstOrDefault(oh => oh.DayOfWeek == day);
        return hours != null && !hours.IsClosed && hours.OpeningTime.IsWithinRange(time);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
