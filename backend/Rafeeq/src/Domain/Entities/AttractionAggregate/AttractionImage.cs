using Domain.Common;
using Domain.Exceptions;
using Shared.Models;

namespace Domain.Entities.AttractionAggregate;

public class AttractionImage : BaseAuditableEntity
{
    public string ImageUrl { get; private set; } = null!;
    public string? Caption { get; private set; }
    public bool IsMain { get; private set; }
    public int DisplayOrder { get; private set; }
    
    private AttractionImage() { }
    private AttractionImage(string imageUrl, bool isMain, string? caption)
    {
        ImageUrl = imageUrl;
        IsMain = isMain;
        Caption = caption;

        DisplayOrder = 0;
    }

    internal static Result<AttractionImage> Create(string imageUrl, bool isMain, string? caption)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return AttractionErrors.ImageUrlRequired;

        return new AttractionImage(imageUrl, isMain, caption?.Trim());
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
