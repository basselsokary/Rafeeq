using Domain.Common;
using Shared.Models;

namespace Domain.Entities.AttractionAggregate;

public class AttractionImage : BaseAuditableEntity
{
    public string ImageUrl { get; private set; } = null!;
    public string? Caption { get; private set; }
    public bool IsMain { get; private set; }
    public int DisplayOrder { get; private set; }
    
    private AttractionImage() { }
    private AttractionImage(string imageUrl, bool isMain, int displayOrder, string? caption)
    {
        ImageUrl = imageUrl;
        IsMain = isMain;
        Caption = caption;
        DisplayOrder = displayOrder;
    }

    internal static Result<AttractionImage> Create(string imageUrl, bool isMain, int displayOrder, string? caption)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return AttractionErrors.ImageUrlRequired;

        if (displayOrder < 0)
            return AttractionErrors.NegativeDisplayOrder;

        return new AttractionImage(imageUrl, isMain, displayOrder, caption?.Trim());
    }

    internal void SetAsMain(bool isMain)
    {
        IsMain = isMain;
        MarkAsUpdated();
    }

    internal void UpdateCaption(string? caption)
    {
        Caption = caption?.Trim();
        MarkAsUpdated();
    }

    internal Result SetDisplayOrder(int order)
    {
        if (order < 0)
            return AttractionErrors.NegativeDisplayOrder;

        DisplayOrder = order;
        MarkAsUpdated();

        return Result.Success();
    }
}
