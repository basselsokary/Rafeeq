using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Enums;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.AttractionAggregate;

public class Attraction : BaseAuditableEntity, IAggregateRoot
{
    public Guid SiteId { get; private set; }

    public AttractionType Type { get; private set; }
    public bool IsFeatured { get; private set; }

    public GeoLocation? Location { get; private set; } // Specific GPS if available
    public string? MainImageUrl { get; private set; }

    private readonly List<HistoricalPeriod> _historicalPeriods = [];
    public IReadOnlyCollection<HistoricalPeriod> HistoricalPeriods => _historicalPeriods.AsReadOnly();

    private readonly List<AttractionImage> _images = [];
    public IReadOnlyCollection<AttractionImage> Images => _images.AsReadOnly();

    private readonly List<AttractionLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<AttractionLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();

    private Attraction() { }
    private Attraction(
        Guid siteId,
        AttractionType type,
        List<HistoricalPeriod> historicalPeriod)
    {
        SiteId = siteId;
        Type = type;
        _historicalPeriods.AddRange(historicalPeriod);
    }

    public static Result<Attraction> Create(
        Guid siteId,
        string name,
        string description,
        string? locationDescription,
        AttractionType type,
        List<HistoricalPeriod> historicalPeriods)
    {
        var attraction = new Attraction(
            siteId,
            type,
            historicalPeriods);
        
        var result = attraction.AddLocalizedContent(LanguageCode.English, name.Trim(), description.Trim(), locationDescription?.Trim());
        if (result.Failed)
            return result.To<Attraction>();
        
        attraction.RaiseDomainEvent(new AttractionCreatedEvent(attraction.Id));

        return attraction;
    }

    public Result UpdateBasicInfo(
        AttractionType type,
        List<HistoricalPeriod> historicalPeriods)
    {
        if (Type != type)
            Type = type;

        if (_historicalPeriods.Count == historicalPeriods.Count &&
            !_historicalPeriods.Except(historicalPeriods).Any())
        {
            return Result.Success();
        }

        if (_historicalPeriods.Any(f => historicalPeriods.Contains(f)))
            return AttractionErrors.HistoricalPeriodAlreadyExists;

        _historicalPeriods.AddRange(historicalPeriods);
        RaiseDomainEvent(new AttractionUpdatedEvent(Id));

        return Result.Success();
    }

    public void SetLocation(GeoLocation? exactLocation)
    {
        if (exactLocation != null && exactLocation != Location)
        {
            Location = exactLocation;
            RaiseDomainEvent(new AttractionUpdatedEvent(Id));
        }
    }

    public void SetAsFeatured(bool isFeatured)
    {
        if (IsFeatured == isFeatured)
            return;

        IsFeatured = isFeatured;
        RaiseDomainEvent(new AttractionUpdatedEvent(Id));
    }

    public Result AddHistoricalPeriods(List<HistoricalPeriod> facilityTypes)
    {
        if (_historicalPeriods.Any(f => facilityTypes.Contains(f)))
            return AttractionErrors.HistoricalPeriodAlreadyExists;
        
        _historicalPeriods.Clear();
        _historicalPeriods.AddRange(facilityTypes);

        RaiseDomainEvent(new AttractionUpdatedEvent(Id));
        return Result.Success();
    }
    
    public void RemoveHistoricalPeriods(List<HistoricalPeriod> facilityTypes)
    {
        _historicalPeriods.RemoveAll(f => facilityTypes.Contains(f));
        RaiseDomainEvent(new AttractionUpdatedEvent(Id));
    }

    public Result<AttractionImage> AddImage(Guid storedFileId, StorageKey storageKey, string imageUrl, bool isMain, int displayOrder, string? caption = null)
    {
        var imageResult = AttractionImage.Create(storedFileId, storageKey, imageUrl, isMain, displayOrder, caption);
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

        RaiseDomainEvent(new AttractionImageUpdatedEvent(Id));
        return Result.Success(imageResult.Value);
    }

    public Result<AttractionImage> RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            return AttractionErrors.ImageNotFound;

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

        RaiseDomainEvent(new AttractionImageUpdatedEvent(Id));
        return Result.Success(image);
    }

    public Result SetMainImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            return AttractionErrors.ImageNotFound;

        var mainImages = _images.Where(i => i.IsMain == true).ToList();
        foreach (var img in mainImages)
            img.SetAsMain(false);

        image.SetAsMain(true);
        SetMainImage(image.ImageUrl);

        RaiseDomainEvent(new AttractionImageUpdatedEvent(Id));
        return Result.Success();
    }

    public Result<AttractionLocalizedContent> AddLocalizedContent(LanguageCode language, string name, string description, string? locationDescription)
    {
        var contentResult = AttractionLocalizedContent.Create(language, name, description, locationDescription);
        if (contentResult.Failed)
            return contentResult;

        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        _localizedContents.Add(contentResult.Value);

        RaiseDomainEvent(new AttractionLocalizedContentUpdatedEvent(Id));

        return Result.Success(contentResult.Value);
    }

    public Result UpdateLocalizedContent(LanguageCode language, string name, string description, string? locationDescription)
    {
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing == null)
            return AttractionErrors.LocalizedContentNotFound;

        Result result = existing.Update(name, description, locationDescription);
        if (result.Failed)
            return result;

        RaiseDomainEvent(new AttractionLocalizedContentUpdatedEvent(Id));

        return Result.Success();
    }

    private void SetMainImage(string imageUrl)
    {
        if (MainImageUrl != imageUrl)
        {
            MainImageUrl = imageUrl;
        }
    }

    public void Delete()
    {
        RaiseDomainEvent(new AttractionDeletedEvent(Id));
    }
}
