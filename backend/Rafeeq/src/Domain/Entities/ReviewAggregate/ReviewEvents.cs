using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities.ReviewAggregate;

public class ReviewCreatedEvent(Guid ReviewId, Guid SiteId, Guid UserId, Rating Rating) : BaseEvent
{
    public Guid ReviewId { get; } = ReviewId;
    public Guid SiteId { get; } = SiteId;
    public Guid UserId { get; } = UserId;
    public Rating Rating { get; } = Rating;
}

public class ReviewApprovedEvent(Guid ReviewId, Guid SiteId, Guid UserId, Rating Rating) : BaseEvent
{
    public Guid ReviewId { get; } = ReviewId;
    public Guid SiteId { get; } = SiteId;
    public Guid UserId { get; } = UserId;
    public Rating Rating { get; } = Rating;
}

public class ReviewUpdatedEvent(Guid ReviewId, Guid SiteId, Rating OldRating, Rating NewRating) : BaseEvent
{
    public Guid ReviewId { get; } = ReviewId;
    public Guid SiteId { get; } = SiteId;
    public Rating OldRating { get; } = OldRating;
    public Rating NewRating { get; } = NewRating;
}

public class ReviewDeletedEvent(Guid ReviewId, Guid SiteId, Rating Rating) : BaseEvent
{
    public Guid ReviewId { get; } = ReviewId;
    public Guid SiteId { get; } = SiteId;
    public Rating Rating { get; } = Rating;
}
