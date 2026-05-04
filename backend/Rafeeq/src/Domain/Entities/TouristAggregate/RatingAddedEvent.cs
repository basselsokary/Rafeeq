using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities.TouristAggregate;

public sealed class RatingAddedEvent(Guid VisitedSiteId, Guid SiteId, Guid UserId, Rating Rating) : BaseEvent
{
    public Guid VisitedSiteId { get; } = VisitedSiteId;
    public Guid SiteId { get; } = SiteId;
    public Guid UserId { get; } = UserId;
    public Rating Rating { get; } = Rating;
}
