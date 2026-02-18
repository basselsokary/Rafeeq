using Domain.Common;
using Domain.Common.Exceptions;

namespace Domain.Entities.SponsorAggregate;

public class SponsorImage : BaseAuditableEntity
{
    public string ImageUrl { get; private set; } = null!;
    public bool IsMain { get; private set; }
    public string? Caption { get; private set; }
    public int DisplayOrder { get; private set; }

    private SponsorImage() { }
    private SponsorImage(string imageUrl, bool isMain, string? caption)
    {
        ImageUrl = imageUrl;
        IsMain = isMain;
        Caption = caption;
        DisplayOrder = 0;
    }

    internal static SponsorImage Create(string imageUrl, bool isMain, string? caption = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new BusinessRuleValidationException("Image URL cannot be empty.");

        return new SponsorImage(imageUrl.Trim(), isMain, caption?.Trim());
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

    internal void SetDisplayOrder(int order)
    {
        if (order < 0)
            throw new BusinessRuleValidationException("Display order cannot be negative.");

        DisplayOrder = order;
        MarkAsUpdated();
    }
}
