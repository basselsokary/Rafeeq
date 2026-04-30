using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities.TouristAggregate;

public sealed class RatingRemovedEvent(Guid VisitedSiteId, Guid SiteId, Rating Rating) : BaseEvent
{
    public Guid VisitedSiteId { get; } = VisitedSiteId;
    public Guid SiteId { get; } = SiteId;
    public Rating Rating { get; } = Rating;
}
