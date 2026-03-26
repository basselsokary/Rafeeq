using Domain.Common;
using Shared.Models;

namespace Domain.Entities.SiteAggregate;

public class SiteImage : BaseAuditableEntity
{
    public string ImageUrl { get; private set; } = null!;
    public string? Caption { get; private set; }
    public bool IsMain { get; private set; }
    public int DisplayOrder { get; private set; }
    
    private SiteImage() { }
    private SiteImage(string imageUrl, bool isMain, int displayOrder, string? caption)
    {
        ImageUrl = imageUrl;
        IsMain = isMain;
        DisplayOrder = displayOrder;
        Caption = caption;
    }

    internal static Result<SiteImage> Create(string imageUrl, bool isMain, int displayOrder, string? caption)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return SiteErrors.ImageUrlRequired;
        
        if (displayOrder < 0)
            return SiteErrors.NegativeDisplayOrder;

        return new SiteImage(imageUrl, isMain, displayOrder, caption?.Trim());
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
            return SiteErrors.NegativeDisplayOrder;

        DisplayOrder = order;
        MarkAsUpdated();

        return Result.Success();
    }
}