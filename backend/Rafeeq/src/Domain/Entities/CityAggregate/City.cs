using Domain.Common;
using Domain.Common.Interfaces;
using Domain.ValueObjects;
using Domain.Enums;
using Shared;

namespace Domain.Entities.CityAggregate;

public class City : BaseAuditableEntity, IAggregateRoot
{
    public GeoLocation CenterLocation { get; private set; } = null!;
    
    public Guid? StoredFileId { get; private set; }
    public StorageKey? StorageKey { get; private set; }
    public string? ImageUrl { get; private set; }
    public int TotalSites { get; private set; }
    public int DisplayOrder { get; private set; }

    private readonly List<CityLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<CityLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();

    private City() { }
    private City(Guid? storedFileId, GeoLocation centerLocation, StorageKey? storageKey, string? imageUrl, int displayOrder)
    {
        StoredFileId = storedFileId;
        CenterLocation = centerLocation;
        StorageKey = storageKey;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;

        TotalSites = 0;
    }

    public static Result<City> Create(
        Guid storedFileId,
        string name,
        string description,
        GeoLocation centerLocation,
        StorageKey storageKey,
        string imageUrl,
        int displayOrder)
    {
        var city = new City(storedFileId, centerLocation, storageKey, imageUrl, displayOrder);
    
        var contentResult = city.AddLocalizedContent(LanguageCode.English, name, description);
        if (contentResult.Failed)
            return contentResult.To<City>();
        
        city.RaiseDomainEvent(new CityCreatedEvent(city.Id, name));

        return city;
    }

    public static Result<City> Create(
        string name,
        string description,
        GeoLocation centerLocation,
        int displayOrder)
    {
        var city = new City(null, centerLocation, null, string.Empty, displayOrder);
    
        var contentResult = city.AddLocalizedContent(LanguageCode.English, name, description);
        if (contentResult.Failed)
            return contentResult.To<City>();
        
        city.RaiseDomainEvent(new CityCreatedEvent(city.Id, name));

        return city;
    }

    public void SetCenterLocation(GeoLocation location)
    {
        if (CenterLocation != location)
        {
            CenterLocation = location;
            RaiseDomainEvent(new CityUpdatedEvent(Id));
        }
    }

    public Result SetImage(Guid storedFileId, StorageKey storageKey, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return CityErrors.ImageUrlRequired;

        StoredFileId = storedFileId;
        StorageKey = storageKey;
        ImageUrl = imageUrl.Trim();

        RaiseDomainEvent(new CityUpdatedEvent(Id));
        return Result.Success();
    }

    public Result SetDisplayOrder(int order)
    {
        if (order < 0)
            return CityErrors.NegativeDisplayOrder;

        DisplayOrder = order;

        RaiseDomainEvent(new CityUpdatedEvent(Id));
        return Result.Success();
    }

    public void IncrementSiteCount()
    {
        TotalSites++;
        RaiseDomainEvent(new CityUpdatedEvent(Id));
    }

    public void DecrementSiteCount()
    {
        if (TotalSites > 0)
        {
            TotalSites--;
            RaiseDomainEvent(new CityUpdatedEvent(Id));
        }
    }

    public Result<CityLocalizedContent> AddLocalizedContent(LanguageCode language, string name, string description)
    {
        var contentResult = CityLocalizedContent.Create(language, name, description);
        if (contentResult.Failed)
            return contentResult;

        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        _localizedContents.Add(contentResult.Value);

        RaiseDomainEvent(new CityLocalizedContentUpdatedEvent(Id));

        return contentResult.Value;
    }

    public Result<CityLocalizedContent> UpdateLocalizedContent(LanguageCode language, string name, string description)
    {
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing == null)
            return CityErrors.LocalizedContentNotFound;

        var result = existing.Update(name, description);
        if (result.Failed)
            return result;

        RaiseDomainEvent(new CityLocalizedContentUpdatedEvent(Id));

        return result.Value;
    }

    public void Delete()
    {
        RaiseDomainEvent(new CityDeletedEvent(Id));
    }
}