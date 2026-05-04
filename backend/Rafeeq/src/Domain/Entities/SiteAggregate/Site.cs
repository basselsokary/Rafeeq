using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.Enums;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.SiteAggregate;

public class Site : BaseAuditableEntity, IAggregateRoot
{
    public Guid CityId { get; private set; }
    /// Read only
    public City City { get; private set; } = null!;

    public SiteStatus Status { get; private set; }
    public SiteType Type { get; private set; }
    public GeoLocation Location { get; private set; } = null!;

    public Ticket? EntryTicket { get; private set; }
    public bool IsFree { get; private set; }

    public string? WebsiteUrl { get; private set; }
    public string? MainImageUrl { get; private set; }
    public string? ContactPhone { get; private set; }

    public int EstimatedDurationMinutes { get; private set; }
    public int TotalVisits { get; private set; }
    public int TotalRating { get; private set; }
    public double AverageRating { get; private set; }
    
    /// IsFeatured and IsHiddenGem are mutually exclusive, but we allow both to be false for sites that are neither.
    public bool IsFeatured { get; private set; }
    public bool IsHiddenGem { get; private set; }
    public bool IsPopular { get; private set; }

    private readonly List<NearestTransportation> _nearestTransportations = [];
    public IReadOnlyCollection<NearestTransportation> NearestTransportations => _nearestTransportations.AsReadOnly();

    private readonly List<SiteImage> _images = [];
    public IReadOnlyCollection<SiteImage> Images => _images.AsReadOnly();

    private readonly List<SiteLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<SiteLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();

    private readonly List<FacilityType> _facilities = [];
    public IReadOnlyCollection<FacilityType> Facilities => _facilities.AsReadOnly();

    private readonly List<OpeningHour> _openingHours = [];
    public IReadOnlyCollection<OpeningHour> OpeningHours => _openingHours.AsReadOnly();

    private Site() { }
    private Site(
        Guid cityId,
        GeoLocation location,
        SiteType type,
        int estimatedDurationMinutes,
        string? contactPhone,
        string? contactWebsiteUrl)
    {
        CityId = cityId;

        Location = location;
        Type = type;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        ContactPhone = contactPhone;
        WebsiteUrl = contactWebsiteUrl;

        Status = SiteStatus.Inactive;
        IsFree = false;
        
        IsFeatured = false;
        IsHiddenGem = false;
        AverageRating = 0.0;
        TotalRating = 0;
        TotalVisits = 0;
    }

    public static Result<Site> Create(
        Guid cityId,
        string name,
        string description,
        Address address,
        string? entryFeeNotes,
        GeoLocation location,
        SiteType type,
        int estimatedDurationMinutes,
        string? contactPhone,
        string? contactWebsiteUrl)
    {
        if (cityId == Guid.Empty)
            return SiteErrors.CityIdRequired;
        
        if (estimatedDurationMinutes <= 0)
            return SiteErrors.InvalidEstimatedDuration;
        
        var site = new Site(cityId, location, type, estimatedDurationMinutes, contactPhone, contactWebsiteUrl);

        var addResult = site.AddLocalizedContent(LanguageCode.English, name, description, address, entryFeeNotes);
        if (addResult.Failed)
            return addResult.To<Site>();
        
        site.UpdatePopularityStatus();

        return site;
    }

    public Result UpdateBasicInfo(SiteType type, int estimatedDurationMinutes)
    {
        if (estimatedDurationMinutes <= 0)
            return Result.Failure(SiteErrors.InvalidEstimatedDuration);

        Type = type;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        return Result.Success();
    }

    public void UpdateLocation(GeoLocation location)
    {
        if (location != Location)
        {
            Location = location;
        }
    }

    public void SetEntryFee(Ticket entryTicket)
    {   
        if (entryTicket != EntryTicket)
        {
            EntryTicket = entryTicket;
            if (EntryTicket.EgyptianPrice?.Amount == 0 && EntryTicket.ForeignerPrice?.Amount == 0)
                IsFree = true;
            else
                IsFree = false;
        }
    }

    public void RemoveEntryFee(bool isFree = false)
    {
        EntryTicket = null;
        IsFree = isFree;
    }

    public Result SetContactInfo(string phone, string? websiteUrl)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return SiteErrors.PhoneRequired;

        ContactPhone = phone;
        WebsiteUrl = websiteUrl;

        return Result.Success();
    }

    public Result UpdateStatus(SiteStatus status, bool isHiddenGem, bool isFeatured)
    {
        if (isHiddenGem && isFeatured)
            return Result.Failure(SiteErrors.CannotBeFeaturedAndHiddenGem);

        if (IsHiddenGem != isHiddenGem)
            IsHiddenGem = isHiddenGem;
        
        if (IsFeatured != isFeatured)
            IsFeatured = isFeatured;
        
        if (Status != status)
        {
            Status = status;
        }

        return Result.Success();
    }

    public void AddRating(Rating rating)
    {
        var totalScore = AverageRating * TotalRating;
        TotalRating++;
        AverageRating = (totalScore + rating) / TotalRating;
        UpdatePopularityStatus();
    }

    public void RemoveRating(Rating rating)
    {
        if (TotalRating == 0)
            return;

        var totalScore = AverageRating * TotalRating;
        TotalRating--;
        AverageRating = TotalRating > 0 ? (totalScore - rating) / TotalRating : 0.0;
        UpdatePopularityStatus();
    }

    public void UpdateRating(Rating oldRating, Rating newRating)
    {
        if (TotalRating == 0 || oldRating == newRating)
            return;

        var totalScore = AverageRating * TotalRating;
        AverageRating = (totalScore - oldRating + newRating) / TotalRating;
        UpdatePopularityStatus();
    }

    public Result<NearestTransportation> AddNearestTransportation(
        TransportationType type,
        GeoLocation location,
        double distanceKm)
    {
        var transportationResult = NearestTransportation.Create(type, location, distanceKm);
        if (transportationResult.Failed)
            return transportationResult;

        var transportation = _nearestTransportations.FirstOrDefault(t => t.Location == location);
        if (transportation != null)
            return SiteErrors.TransportationWithSameLocationAlreadyExists;

        _nearestTransportations.Add(transportationResult.Value);

        return Result.Success(transportationResult.Value);
    }

    public Result RemoveNearestTransportation(Guid transportationId)
    {
        var transportation = _nearestTransportations.FirstOrDefault(t => t.Id == transportationId);
        if (transportation == null)
            return SiteErrors.TransportationNotFound;

        _nearestTransportations.Remove(transportation);

        return Result.Success();
    }

    public Result<OpeningHour> AddOpeningHour(WeekDay day, TimeRange openingTime, bool isClosed)
    {
        var newOpeningHours = OpeningHour.Create(day, openingTime, isClosed);

        var existing = _openingHours.FirstOrDefault(oh => oh.Day == day);
        if (existing != null)
        {
            if (existing.Equals(newOpeningHours))
                return existing; // No changes, so we can skip the update

            _openingHours.Remove(existing);
        }

        _openingHours.Add(newOpeningHours);

        return newOpeningHours;
    }

    public Result RemoveOpeningHour(WeekDay day)
    {
        var openingHour = _openingHours.FirstOrDefault(t => t.Day == day);
        if (openingHour == null)
            return SiteErrors.OpeningHourNotFound;

        _openingHours.Remove(openingHour);

        return Result.Success();
    }

    public Result<SiteImage> AddImage(StorageKey storageKey, string imageUrl, bool isMain, int displayOrder, string? caption = null)
    {
        var imageResult = SiteImage.Create(storageKey, imageUrl, isMain, displayOrder, caption);
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

        return Result.Success();
    }

    public Result<SiteLocalizedContent> AddLocalizedContent(LanguageCode language, string name, string description, Address address, string? entryFeeNotes)
    {
        var contentResult = SiteLocalizedContent.Create(language, name, description, address, entryFeeNotes);
        if (contentResult.Failed)
            return contentResult;

        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        _localizedContents.Add(contentResult.Value);

        return Result.Success(contentResult.Value);
    }

    public Result UpdateLocalizedContent(LanguageCode language, string name, string description, Address address, string? entryFeeNotes)
    {
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing == null)
            return SiteErrors.LocalizedContentNotFound;

        Result result = existing.Update(name, description, entryFeeNotes);
        if (result.Failed)
            return result;

        existing.UpdateAddress(address);
        
        return Result.Success();
    }

    public Result AddFacilities(List<FacilityType> facilityTypes)
    {
        if (_facilities.Count == facilityTypes.Count &&
            !_facilities.Except(facilityTypes).Any())
        {
            return Result.Success();
        }
        
        if (_facilities.Any(f => facilityTypes.Contains(f)))
            return SiteErrors.FacilityAlreadyExists;
        
        _facilities.AddRange(facilityTypes);

        return Result.Success();
    }
    
    public void RemoveFacilities(List<FacilityType> facilityTypes)
    {
        _facilities.RemoveAll(f => facilityTypes.Contains(f));
    }

    public bool IsOpenAt(WeekDay day, TimeSpan time)
    {
        var hours = _openingHours.FirstOrDefault(oh => oh.Day == day);
        return hours != null && !hours.IsClosed && hours.OpeningTime.IsWithinRange(time);
    }

    public void IncrementVisitCount()
    {
        TotalVisits++;
        UpdatePopularityStatus();
    }
    
    private void UpdatePopularityStatus()
    {
        const int popularityThreshold = 1000;
        IsPopular = TotalVisits >= popularityThreshold || AverageRating >= 4.5;
    }

    private void SetMainImage(string imageUrl)
    {
        if (MainImageUrl != imageUrl)
        {
            MainImageUrl = imageUrl;
        }
    }
}
