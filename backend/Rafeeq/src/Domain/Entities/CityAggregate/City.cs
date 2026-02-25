using Domain.Common;
using Domain.Exceptions;
using Domain.Common.Interfaces;
using Domain.ValueObjects;
using Domain.Enums;

namespace Domain.Entities.CityAggregate;

public class City : BaseAuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public GeoLocation CenterLocation { get; private set; } = null!;
    public string? ImageUrl { get; private set; }
    public int TotalAttractions { get; private set; }
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
        TotalAttractions = 0;
    }

    public static City Create(string name, string description, GeoLocation centerLocation)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("City name cannot be null or empty.");
        
        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Description cannot be null or empty.");

        return new City(name.Trim(), description.Trim(), centerLocation);
    }

    public void UpdateBasicInfo(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("City name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Description cannot be null or empty.");

        Name = name.Trim();
        Description = description.Trim();
        MarkAsUpdated();
    }

    public void SetCenterLocation(GeoLocation location)
    {
        CenterLocation = location;
        MarkAsUpdated();
    }

    public void SetImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new BusinessRuleValidationException("Image URL cannot be empty.");

        ImageUrl = imageUrl.Trim();
        MarkAsUpdated();
    }

    public void SetDisplayOrder(int order)
    {
        if (order < 0)
            throw new BusinessRuleValidationException("Display order cannot be negative.");

        DisplayOrder = order;
        MarkAsUpdated();
    }

    public void IncrementAttractionCount()
    {
        TotalAttractions++;
        MarkAsUpdated();
    }

    public void DecrementAttractionCount()
    {
        if (TotalAttractions > 0)
        {
            TotalAttractions--;
            MarkAsUpdated();
        }
    }

    public void AddLocalizedContent(LanguageCode language, string name, string description)
    {
        var content = CityLocalizedContent.Create(language, name, description);
        
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        _localizedContents.Add(content);
        MarkAsUpdated();
    }

    public void UpdateLocalizedContent(Guid contentId, string name, string description)
    {
        var content = _localizedContents.FirstOrDefault(lc => lc.Id == contentId)
            ?? throw new EntityNotFoundException(nameof(CityLocalizedContent), contentId);

        content.Update(name, description);
        MarkAsUpdated();
    }

    public void RemoveLocalizedContent(Guid contentId)
    {
        var content = _localizedContents.FirstOrDefault(lc => lc.Id == contentId)
            ?? throw new EntityNotFoundException(nameof(CityLocalizedContent), contentId);
        
        _localizedContents.Remove(content);
        MarkAsUpdated();
    }

    public string GetLocalizedName(LanguageCode language)
    {
        var localized = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        return localized?.Name ?? Name;
    }
}