using Domain.Common;
using Shared.Models;

namespace Domain.Entities.TouristAggregate;

public class Favourite : BaseAuditableEntity
{
    public Guid SiteId { get; private set; }

    public string? Notes { get; private set; }

    private Favourite() { }
    private Favourite(Guid siteId)
    {
        SiteId = siteId;
    }

    public static Result<Favourite> Create(Guid siteId)
    {
        if (siteId == Guid.Empty)
            return Error.Validation(
                "EMPTY_SITE_ID", "Site ID cannot be empty.");

        return new Favourite(siteId);
    }

    public void AddNotes(string notes)
    {
        Notes = notes?.Trim();
        MarkAsUpdated();
    }
}

