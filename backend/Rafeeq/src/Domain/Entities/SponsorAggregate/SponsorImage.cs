using Domain.Common;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.SponsorAggregate;

public class SponsorImage : BaseAuditableEntity
{
    public StorageKey StorageKey { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public string? Caption { get; private set; }
    public bool IsMain { get; private set; }
    public int DisplayOrder { get; private set; }

    private SponsorImage() { }
    private SponsorImage(StorageKey storageKey, string imageUrl, bool isMain, int displayOrder, string? caption)
    {
        StorageKey = storageKey;

        ImageUrl = imageUrl;
        IsMain = isMain;
        Caption = caption;
        DisplayOrder = displayOrder;
    }

    internal static Result<SponsorImage> Create(StorageKey storageKey, string imageUrl, bool isMain, int displayOrder, string? caption)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            return ImageErrors.StorageKeyRequired;

        if (string.IsNullOrWhiteSpace(imageUrl))
            return ImageErrors.ImageUrlRequired;
        
        if (displayOrder < 0)
            return ImageErrors.NegativeDisplayOrder;

        return new SponsorImage(storageKey, imageUrl, isMain, displayOrder, caption?.Trim());
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