using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Enums;
using Domain.ValueObjects;
using Shared.Models;

namespace Domain.Entities.SiteAggregate;

public class Site : BaseAuditableEntity, IAggregateRoot
{
    public Guid CityId { get; private set; }
    public string CityName { get; private set; } = null!;
    
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

    private readonly List<NearestTransportation> _nearestTransportations = [];
    public IReadOnlyCollection<NearestTransportation> NearestTransportations => _nearestTransportations.AsReadOnly();
    
    private readonly List<SiteImage> _images = [];
    public IReadOnlyCollection<SiteImage> Images => _images.AsReadOnly();
    
    private readonly List<SiteLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<SiteLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();
    
    private readonly List<Facility> _facilities = [];
    public IReadOnlyCollection<Facility> Facilities => _facilities.AsReadOnly();

    private readonly List<OpeningHour> _openingHours = [];
    public IReadOnlyCollection<OpeningHour> OpeningHours => _openingHours.AsReadOnly();
    
    private Site() { }
    private Site(
        Guid cityId,
        string cityName,
        string name,
        string description,
        GeoLocation location,
        Address address,
        SiteType type)
    {
        CityId = cityId;
        CityName = cityName;
        Name = name;
        Description = description;
        Location = location;
        Address = address;
        Type = type;
        
        IsActive = false;
        AverageRating = 0.0;
        TotalReviews = 0;
    }

    public static Result<Site> Create(
        Guid cityId,
        string cityName,
        string name,
        string description,
        GeoLocation location,
        Address address,
        SiteType type)
    {
        if (cityId == Guid.Empty)
            return SiteErrors.CityIdRequired;
        
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return SiteErrors.DescriptionRequired;
        
        return new Site(cityId, cityName, name, description, location, address, type);
    }

    public Result UpdateBasicInfo(string name, string description, SiteType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            return SiteErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return SiteErrors.DescriptionRequired;

        Name = name.Trim();
        Description = description.Trim();
        Type = type;
        MarkAsUpdated();

        return Result.Success();
    }

    public void UpdateLocation(GeoLocation location, Address address)
    {
        Location = location;
        Address = address;
        MarkAsUpdated();
    }

    public void UpdateCity(Guid cityId, string cityName)
    {
        CityId = cityId;
        CityName = cityName;
        MarkAsUpdated();
    }

    public Result AddOpeningHours(DayOfWeek dayOfWeek, TimeRange openingTime, bool isClosed)
    {
        if (_openingHours.Any(o => o.DayOfWeek == dayOfWeek))
            return OpeningHourErrors.AlreadyExist(dayOfWeek);

        var openingHours = OpeningHour.Create(dayOfWeek, openingTime, isClosed);
        _openingHours.Add(openingHours);
        
        return Result.Success();
    }

    public Result UpdateOpeningHours(DayOfWeek dayOfWeek, TimeRange openingTime, bool isClosed)
    {
        var existing = _openingHours.FirstOrDefault(oh => oh.DayOfWeek == dayOfWeek);
        if (existing == null)
            return OpeningHourErrors.NotFound(dayOfWeek);

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

    public Result AddImage(string imageUrl, bool isMain, string? caption = null)
    {
        var imageResult = SiteImage.Create(imageUrl, isMain, caption);
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
            return SiteErrors.ImageNotFound;

        _images.Remove(image);
        MarkAsUpdated();

        return Result.Success();
    }

    public Result AddLocalizedContent(LanguageCode language, string name, string description)
    {
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        var contentResult = SiteLocalizedContent.Create(language, name, description);
        if (contentResult.Failed)
            return contentResult;

        _localizedContents.Add(contentResult.Value);
        MarkAsUpdated();

        return Result.Success();
    }

    public Result AddFacility(string name, string description)
    {
        var facilityResult = Facility.Create(name, description);
        if (facilityResult.Failed)
            return facilityResult;

        _facilities.Add(facilityResult.Value);
        MarkAsUpdated();

        return Result.Success();
    }

    public Result RemoveFacility(Guid facilityId)
    {
        var facility = _facilities.FirstOrDefault(f => f.Id == facilityId);
        if (facility == null)
            return SiteErrors.FacilityNotFound;

        _facilities.Remove(facility);
        MarkAsUpdated();

        return Result.Success();
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
