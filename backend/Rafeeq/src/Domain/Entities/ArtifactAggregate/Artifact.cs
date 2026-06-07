using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Shared;

namespace Domain.Entities.ArtifactAggregate;

public class Artifact : BaseAuditableEntity, IAggregateRoot
{
    public Guid? SiteId { get; private set; }
    public Site Site { get; private set; } = null!;

    public string? MainImageUrl { get; private set; }
    public int DisplayOrder { get; private set; }
    public ArtifactType Type { get; private set; }
    
    private readonly List<ArtifactImage> _images = [];
    public IReadOnlyCollection<ArtifactImage> Images => _images.AsReadOnly();

    private readonly List<ArtifactLocalizedContent> _localizedContents = [];
    public IReadOnlyCollection<ArtifactLocalizedContent> LocalizedContents => _localizedContents.AsReadOnly();

    private Artifact() { }
    private Artifact(Guid? siteId, int displayOrder, ArtifactType type = ArtifactType.Other)
    {
        SiteId = siteId;
        DisplayOrder = displayOrder;
        Type = type;
    }

    public static Result<Artifact> Create(
        Guid? siteId,
        string name,
        string description,
        int displayOrder,
        ArtifactType type)
    {
        if (displayOrder < 0)
            return ArtifactErrors.NegativeDisplayOrder;
        
        var artifact = new Artifact(siteId, displayOrder, type);
        var result = artifact.AddLocalizedContent(LanguageCode.English, name.Trim(), description.Trim());
        if (result.Failed)
            return result.To<Artifact>();
        
        artifact.RaiseDomainEvent(new ArtifactCreatedEvent(artifact.Id));

        return artifact;
    }

    public Result Update(int displayOrder, ArtifactType type)
    {
        if (displayOrder <= 0)
            return ArtifactErrors.NegativeDisplayOrder;

        DisplayOrder = displayOrder;
        Type = type;
        RaiseDomainEvent(new ArtifactUpdatedEvent(Id));
        return Result.Success();
    }

    public void AssignSite(Guid? siteId)
    {
        RaiseDomainEvent(new ArtifactUpdatedEvent(Id));
        SiteId = siteId;
    }

    public Result<ArtifactImage> AddImage(string storageKey, string imageUrl, bool isMain, int displayOrder, string? caption = null)
    {
        var imageResult = ArtifactImage.Create(storageKey, imageUrl, isMain, displayOrder, caption);
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

        RaiseDomainEvent(new ArtifactImageUpdatedEvent(Id));
        return Result.Success(imageResult.Value);
    }

    public Result<ArtifactImage> RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            return ArtifactErrors.ImageNotFound;

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

        RaiseDomainEvent(new ArtifactImageUpdatedEvent(Id));
        return Result.Success(image);
    }

    public Result SetMainImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            return ArtifactErrors.ImageNotFound;

        var mainImages = _images.Where(i => i.IsMain).ToList();
        foreach (var img in mainImages)
            img.SetAsMain(false);

        image.SetAsMain(true);
        SetMainImage(image.ImageUrl);

        RaiseDomainEvent(new ArtifactImageUpdatedEvent(Id));
        return Result.Success();
    }

    public Result<ArtifactLocalizedContent> AddLocalizedContent(LanguageCode language, string name, string description)
    {
        var contentResult = ArtifactLocalizedContent.Create(language, name, description);
        if (contentResult.Failed)
            return contentResult;

        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing != null)
            _localizedContents.Remove(existing);

        _localizedContents.Add(contentResult.Value);

        RaiseDomainEvent(new ArtifactLocalizedContentUpdatedEvent(Id));
        return Result.Success(contentResult.Value);
    }

    public Result UpdateLocalizedContent(LanguageCode language, string name, string description)
    {
        var existing = _localizedContents.FirstOrDefault(lc => lc.Language == language);
        if (existing == null)
            return ArtifactErrors.LocalizedContentNotFound;

        Result result = existing.Update(name, description);
        if (result.Failed)
            return result;

        RaiseDomainEvent(new ArtifactLocalizedContentUpdatedEvent(Id));
        return Result.Success();
    }

    private void SetMainImage(string imageUrl)
    {
        if (MainImageUrl != imageUrl)
        {
            MainImageUrl = imageUrl;
        }
    }
}