using Domain.Common;
using Shared;

namespace Domain.Entities.TouristAggregate;

public class FavouriteSite : BaseAuditableEntity
{
    public Guid SiteId { get; private set; }

    public string? Notes { get; private set; }

    private FavouriteSite() { }
    private FavouriteSite(Guid siteId)
    {
        SiteId = siteId;
    }

    public static Result<FavouriteSite> Create(Guid siteId)
    {
        if (siteId == Guid.Empty)
            return TouristErrors.RequiredSiteId;

        return new FavouriteSite(siteId);
    }

    public void AddNotes(string notes)
    {
        Notes = notes?.Trim();
    }
}
