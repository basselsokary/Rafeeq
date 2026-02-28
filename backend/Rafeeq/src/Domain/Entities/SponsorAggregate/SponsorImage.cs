using Domain.Common;
using Shared.Models;

namespace Domain.Entities.SponsorAggregate;

public class SponsorImage : BaseAuditableEntity
{
    public string ImageUrl { get; private set; } = null!;
    public string? Caption { get; private set; }
    public bool IsMain { get; private set; }
    public int DisplayOrder { get; private set; }

    private SponsorImage() { }
    private SponsorImage(string imageUrl, bool isMain, string? caption)
    {
        ImageUrl = imageUrl;
        IsMain = isMain;
        Caption = caption;
        DisplayOrder = 0;
    }

    internal static Result<SponsorImage> Create(string imageUrl, bool isMain, string? caption)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return SponsorErrors.ImageUrlRequired;

        return new SponsorImage(imageUrl, isMain, caption?.Trim());
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
            return SponsorErrors.NegativeDisplayOrder;

        DisplayOrder = order;
        MarkAsUpdated();

        return Result.Success();
    }
}
