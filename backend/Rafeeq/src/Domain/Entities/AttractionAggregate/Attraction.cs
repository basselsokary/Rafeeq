using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Enums;
using Domain.ValueObjects;
using Shared.Models;

namespace Domain.Entities.AttractionAggregate;

public class Attraction : BaseAuditableEntity, IAggregateRoot
{
    public Guid SiteId { get; private set; }

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public AttractionType Type { get; private set; }
    public HistoricalPeriod HistoricalPeriod { get; private set; }
    public GeoLocation? Location { get; private set; } // Specific GPS if available
    public string? LocationDescription { get; private set; } // e.g., "North side of main hall"
    
    private readonly List<AttractionImage> _images = [];
    public IReadOnlyCollection<AttractionImage> Images => _images.AsReadOnly();
    
    private readonly List<AttractionLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<AttractionLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();

    private Attraction() { }
    private Attraction(
        Guid siteId,
        string name,
        string description,
        AttractionType type,
        HistoricalPeriod historicalPeriod)
    {
        SiteId = siteId;
        Name = name;
        Description = description;
        Type = type;
        HistoricalPeriod = historicalPeriod;
    }

    public static Result<Attraction> Create(
        Guid siteId,
        string name,
        string description,
        AttractionType type,
        HistoricalPeriod historicalPeriod = HistoricalPeriod.Unknown)
    {
        if (string.IsNullOrWhiteSpace(name))
            return AttractionErrors.NameRequired;

        if (string.IsNullOrWhiteSpace(description))
            return AttractionErrors.DescriptionRequired;

        var attraction = new Attraction(
            siteId,
            name.Trim(),
            description.Trim(),
            type,
            historicalPeriod);

        return attraction;
    }

    public Result UpdateBasicInfo(
        string name,
        string description,
        AttractionType type,
        HistoricalPeriod historicalPeriod)
    {
        if (string.IsNullOrWhiteSpace(name))
            return AttractionErrors.NameRequired;

        if (string.IsNullOrWhiteSpace(description))
            return AttractionErrors.DescriptionRequired;

        Name = name.Trim();
        Description = description.Trim();
        Type = type;
        HistoricalPeriod = historicalPeriod;
        MarkAsUpdated();

        return Result.Success();
    }

    public void SetLocation(GeoLocation? exactLocation, string? locationDescription)
    {
        Location = exactLocation;
        LocationDescription = locationDescription?.Trim();
        MarkAsUpdated();
    }

    public Result AddImage(string imageUrl, bool isMain, string? caption = null)
    {
        var imageResult = AttractionImage.Create(imageUrl, isMain, caption);
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
            return AttractionErrors.ImageNotFound;

        _images.Remove(image);
        MarkAsUpdated();

        return Result.Success();
    }

    public Result AddLocalizedContent(LanguageCode language, string name, string description)
    {
        var contentResult = AttractionLocalizedContent.Create(language, name, description);
        if (contentResult.Failed)
            return contentResult;
        
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        _localizedContents.Add(contentResult.Value);
        MarkAsUpdated();

        return Result.Success();
    }

    public Result RemoveLocalizedContent(Guid contentId)
    {
        var content = _localizedContents.FirstOrDefault(lc => lc.Id == contentId);
        if (content == null)
            return AttractionErrors.LocalizedContentNotFound;

        _localizedContents.Remove(content);
        MarkAsUpdated();

        return Result.Success();
    }
}
