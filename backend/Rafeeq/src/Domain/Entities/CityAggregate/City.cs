using Domain.Common;
using Domain.Common.Interfaces;
using Domain.ValueObjects;
using Domain.Enums;
using Shared.Models;

namespace Domain.Entities.CityAggregate;

public class City : BaseAuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public GeoLocation CenterLocation { get; private set; } = null!;
    public string? ImageUrl { get; private set; }
    public int TotalSites { get; private set; }
    public int DisplayOrder { get; private set; }

    private readonly List<CityLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<CityLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();
    
    private City() { }
    private City(string name, string description, GeoLocation centerLocation)
    {
        Name = name;
        CenterLocation = centerLocation;
        Description = description;

        DisplayOrder = 0;
        TotalSites = 0;
    }

    public static Result<City> Create(string name, string description, GeoLocation centerLocation)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CityErrors.NameRequired;
        
        if (string.IsNullOrWhiteSpace(description))
            return CityErrors.DescriptionRequired;
            
        return new City(name.Trim(), description.Trim(), centerLocation);
    }

    public Result UpdateBasicInfo(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CityErrors.NameRequired;
            
        if (string.IsNullOrWhiteSpace(description))
            return CityErrors.DescriptionRequired;
            
        Name = name.Trim();
        Description = description.Trim();
        MarkAsUpdated();

        return Result.Success();
    }

    public void SetCenterLocation(GeoLocation location)
    {
        CenterLocation = location;
        MarkAsUpdated();
    }

    public Result SetImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return CityErrors.ImageUrlRequired;

        ImageUrl = imageUrl.Trim();
        MarkAsUpdated();

        return Result.Success();
    }

    public Result SetDisplayOrder(int order)
    {
        if (order < 0)
            return CityErrors.NegativeDisplayOrder;

        DisplayOrder = order;
        MarkAsUpdated();

        return Result.Success();
    }

    public void IncrementAttractionCount()
    {
        TotalSites++;
        MarkAsUpdated();
    }

    public void DecrementAttractionCount()
    {
        if (TotalSites > 0)
        {
            TotalSites--;
            MarkAsUpdated();
        }
    }

    public Result AddLocalizedContent(LanguageCode language, string name, string description)
    {
        var contentResult = CityLocalizedContent.Create(language, name, description);
        if (contentResult.Failed)
            return contentResult;
        
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        _localizedContents.Add(contentResult.Value);
        MarkAsUpdated();

        return Result.Success();
    }

    public Result UpdateLocalizedContent(Guid contentId, string name, string description)
    {
        var content = _localizedContents.FirstOrDefault(lc => lc.Id == contentId);
        if (content == null)
            return CityErrors.LocalizedContentNotFound;

        var result = content.Update(name, description);
        if (result.Failed)
            return result;

        MarkAsUpdated();

        return Result.Success();
    }

    public Result RemoveLocalizedContent(Guid contentId)
    {
        var content = _localizedContents.FirstOrDefault(lc => lc.Id == contentId);
        if (content == null)
            return CityErrors.LocalizedContentNotFound;

        _localizedContents.Remove(content);
        MarkAsUpdated();

        return Result.Success();
    }

    public string GetLocalizedName(LanguageCode language)
    {
        var localized = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        return localized?.Name ?? Name;
    }
}