using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities.TouristAggregate;

public sealed class RatingUpdatedEvent(
    Guid VisitedSiteId,
    Guid SiteId,
    Rating OldRating,
    Rating NewRating) : BaseEvent
{
    public Guid VisitedSiteId { get; } = VisitedSiteId;
    public Guid SiteId { get; } = SiteId;
    public Rating OldRating { get; } = OldRating;
    public Rating NewRating { get; } = NewRating;
}
