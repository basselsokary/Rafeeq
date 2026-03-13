using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.Enums;
using Domain.ValueObjects;
using Shared.Models;

namespace Domain.Entities.SiteAggregate;

public class Site : BaseAuditableEntity, IAggregateRoot
{
    public Guid CityId { get; private set; }
    /// Read only
    public City City { get; private set; }

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public SiteStatus Status { get; private set; }
    public SiteType Type { get; private set; }
    public Address Address { get; private set; } = null!;
    public GeoLocation Location { get; private set; } = null!;
    
    public Money? EntryFee { get; private set; }
    public bool IsFree { get; private set; }

    public string? WebsiteUrl { get; private set; }
    public string? MainImageUrl { get; private set; }
    public string? ContactPhone { get; private set; }
    public double AverageRating { get; private set; }
    public int TotalReviews { get; private set; }
    public bool IsFeatured { get; private set; }
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
        
        Status = SiteStatus.Active;
        IsActive = false;
        IsFree = false;
        AverageRating = 0.0;
        TotalReviews = 0;
    }

    public static Result<Site> Create(
        Guid cityId,
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
        
        return new Site(cityId, name, description, location, address, type);
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
        if (Location != location || Address != address)
        {
            Location = location;
            Address = address;
            MarkAsUpdated();
        }
    }

    public void UpdateCity(Guid cityId)
    {
        if (CityId != cityId)
        {
            CityId = cityId;
            MarkAsUpdated();
        }
    }

    public void SetEntryFee(Money fee)
    {
        if (fee != EntryFee)
        {
            EntryFee = fee;
            if (EntryFee.Amount == 0)
                IsFree = true;
            else
                IsFree = false;
                
            MarkAsUpdated();
        }
    }

    public void RemoveEntryFee()
    {
        EntryFee = null;
        IsFree = false;
        MarkAsUpdated();
    }

    public Result SetContactInfo(string phone, string? websiteUrl)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return SiteErrors.PhoneRequired;
    
        ContactPhone = phone;
        WebsiteUrl = websiteUrl;
        MarkAsUpdated();

        return Result.Success();
    }

    public void SetStatus(SiteStatus status)
    {
        if (Status != status)
        {
            Status = status;
            MarkAsUpdated();
        }
    }

    public void AddRating(Rating rating)
    {
        var totalScore = AverageRating * TotalReviews;
        TotalReviews++;
        AverageRating = (totalScore + rating) / TotalReviews;

        MarkAsUpdated();
    }

    public void RemoveRating(Rating rating)
    {
        if (TotalReviews == 0)
            return;

        var totalScore = AverageRating * TotalReviews;
        TotalReviews--;
        AverageRating = TotalReviews > 0 ? (totalScore - rating) / TotalReviews : 0.0;

        MarkAsUpdated();
    }

    public void UpdateRating(Rating oldRating, Rating newRating)
    {
        if (TotalReviews == 0)
            return;

        var totalScore = AverageRating * TotalReviews;
        AverageRating = (totalScore - oldRating + newRating) / TotalReviews;

        MarkAsUpdated();
    }

    // public void AddNearestTransportation(string type, string description, double distanceKm)
    // {
    //     var transportation = NearestTransportation.Create(type, description, distanceKm);
    //     _nearestTransportations.Add(transportation);
    //     MarkAsUpdated();
    // }

    public void SetMainImage(string imageUrl)
    {
        if (MainImageUrl != imageUrl)
        {
            MainImageUrl = imageUrl;
            MarkAsUpdated();
        }
    }

    public Result AddOpeningHours(DayOfWeek dayOfWeek, TimeRange openingTime, bool isClosed)
    {
        var newOpeningHours = OpeningHour.Create(dayOfWeek, openingTime, isClosed);
        
        var existing = _openingHours.FirstOrDefault(oh => oh.DayOfWeek == dayOfWeek);
        if (existing != null)
        {
            if (existing.Equals(newOpeningHours))
                return Result.Success(); // No changes, so we can skip the update
            
            _openingHours.Remove(existing);
        }

        _openingHours.Add(newOpeningHours);
        MarkAsUpdated();

        return Result.Success();
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
            
            SetMainImage(imageUrl);
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
        
        MarkAsUpdated();

        return Result.Success();
    }

    public Result AddLocalizedContent(LanguageCode language, string name, string description)
    {
        var contentResult = SiteLocalizedContent.Create(language, name, description);
        if (contentResult.Failed)
            return contentResult;
        
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

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

    public void SetAsFeatured(bool isFeatured)
    {
        IsFeatured = isFeatured;
        MarkAsUpdated();
    }

    public void Activate(bool active)
    {
        IsActive = active;
        MarkAsUpdated();
    }

    public bool IsOpenAt(DayOfWeek day, TimeSpan time)
    {
        var hours = _openingHours.FirstOrDefault(oh => oh.DayOfWeek == day);
        return hours != null && !hours.IsClosed && hours.OpeningTime.IsWithinRange(time);
    }
}
