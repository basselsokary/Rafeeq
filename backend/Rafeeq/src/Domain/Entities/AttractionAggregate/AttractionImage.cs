using Domain.Common;
using Domain.Common.Exceptions;

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

    internal static AttractionImage Create(string imageUrl, bool isMain, string? caption)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new BusinessRuleValidationException("Image URL cannot be null or empty.");

        return new AttractionImage(imageUrl, isMain, caption?.Trim());
    }

    internal void SetAsPrimary(bool isMain)
    {
        IsMain = isMain;
        MarkAsUpdated();
    }

    internal void UpdateCaption(string? caption)
    {
        Caption = caption?.Trim();
        MarkAsUpdated();
    }

    internal void SetDisplayOrder(int order)
    {
        if (order < 0)
            throw new BusinessRuleValidationException("Display order cannot be negative.");

        DisplayOrder = order;
        MarkAsUpdated();
    }
}
