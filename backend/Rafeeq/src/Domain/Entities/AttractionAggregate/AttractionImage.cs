using Domain.Common;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.AttractionAggregate;

public class AttractionImage : BaseAuditableEntity
{
    public Guid StoredFileId { get; private set; }  // FK → StoredFile
    public StorageKey StorageKey { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public string? Caption { get; private set; }
    public bool IsMain { get; private set; }
    public int DisplayOrder { get; private set; }
    
    private AttractionImage() { }
    private AttractionImage(Guid storedFileId, StorageKey storageKey, string imageUrl, bool isMain, int displayOrder, string? caption)
    {
        StoredFileId = storedFileId;
        StorageKey = storageKey;

        ImageUrl = imageUrl;
        IsMain = isMain;
        Caption = caption;
        DisplayOrder = displayOrder;
    }

    public static Result<AttractionImage> Create(Guid storedFileId, StorageKey storageKey, string imageUrl, bool isMain, int displayOrder, string? caption)
    {
        if (storedFileId == Guid.Empty)
            return FileErrors.InvalidId;

        if (string.IsNullOrWhiteSpace(storageKey))
            return ImageErrors.StorageKeyRequired;

        if (string.IsNullOrWhiteSpace(imageUrl))
            return ImageErrors.ImageUrlRequired;

        if (displayOrder < 0)
            return ImageErrors.NegativeDisplayOrder;

        return new AttractionImage(storedFileId, storageKey, imageUrl, isMain, displayOrder, caption?.Trim());
    }

    internal void SetAsMain(bool isMain)
    {
        IsMain = isMain;
    }

    internal Result SetDisplayOrder(int order)
    {
        if (order < 0)
            return ImageErrors.NegativeDisplayOrder;

        DisplayOrder = order;

        return Result.Success();
    }
}