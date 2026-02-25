using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.AttractionAggregate;

public class Attraction : BaseAuditableEntity, IAggregateRoot
{
    public Guid SiteId { get; private set; }

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public AttractionType Type { get; private set; }
    public GeoLocation? Location { get; private set; } // Specific GPS if available
    public string? LocationDescription { get; private set; } // e.g., "North side of main hall"
    public int TotalReviews { get; private set; }
    
    private readonly List<AttractionImage> _images = [];
    public IReadOnlyCollection<AttractionImage> Images => _images.AsReadOnly();
    
    private readonly List<AttractionLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<AttractionLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();

    private Attraction() { }
    private Attraction(
        Guid id,
        Guid siteId,
        string name,
        string description,
        AttractionType type,
        int estimatedViewingTimeMinutes)
    {
        Id = id;
        SiteId = siteId;
        Name = name;
        Description = description;
        Type = type;
        TotalReviews = 0;
    }

    public static Attraction Create(
        Guid siteId,
        string name,
        string description,
        AttractionType type,
        int estimatedViewingTimeMinutes = 5)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Attraction name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Attraction description cannot be empty.");

        if (estimatedViewingTimeMinutes <= 0)
            throw new BusinessRuleValidationException("Estimated viewing time must be greater than zero.");

        var attraction = new Attraction(
            Guid.NewGuid(),
            siteId,
            name.Trim(),
            description.Trim(),
            type,
            estimatedViewingTimeMinutes);

        return attraction;
    }

    public void UpdateBasicInfo(string name, string description, AttractionType type, int estimatedViewingTimeMinutes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Attraction name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Attraction description cannot be empty.");

        if (estimatedViewingTimeMinutes <= 0)
            throw new BusinessRuleValidationException("Estimated viewing time must be greater than zero.");

        Name = name.Trim();
        Description = description.Trim();
        Type = type;
        MarkAsUpdated();
    }

    public void SetLocation(GeoLocation? exactLocation, string? locationDescription)
    {
        Location = exactLocation;
        LocationDescription = locationDescription?.Trim();
        MarkAsUpdated();
    }

    public void AddImage(string imageUrl, bool isPrimary, string? caption = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new BusinessRuleValidationException("Image URL cannot be empty.");

        if (isPrimary)
        {
            foreach (var img in _images)
                img.SetAsMain(false);
        }

        var image = AttractionImage.Create(imageUrl, isPrimary, caption);
        _images.Add(image);
        MarkAsUpdated();
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            throw new EntityNotFoundException(nameof(AttractionImage), imageId);

        _images.Remove(image);
        MarkAsUpdated();
    }

    public void AddLocalizedContent(LanguageCode language, string name, string description)
    {
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        var content = AttractionLocalizedContent.Create(language, name, description);
        _localizedContents.Add(content);
        MarkAsUpdated();
    }
}
