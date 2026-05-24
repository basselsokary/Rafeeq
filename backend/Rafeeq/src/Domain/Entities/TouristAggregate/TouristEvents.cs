using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities.TouristAggregate;

public class TouristProfileUpdatedEvent(Guid TouristId, string FirstName, string LastName) : BaseEvent
{
    public Guid TouristId { get; } = TouristId;
    public string FirstName { get; } = FirstName;
    public string LastName { get; } = LastName;
}

public sealed class RatingAddedEvent(Guid VisitedSiteId, Guid SiteId, Guid UserId, Rating Rating) : BaseEvent
{
    public Guid VisitedSiteId { get; } = VisitedSiteId;
    public Guid SiteId { get; } = SiteId;
    public Guid UserId { get; } = UserId;
    public Rating Rating { get; } = Rating;
}

public sealed class RatingUpdatedEvent(Guid VisitedSiteId, Guid SiteId, Rating OldRating, Rating NewRating) : BaseEvent
{
    public Guid VisitedSiteId { get; } = VisitedSiteId;
    public Guid SiteId { get; } = SiteId;
    public Rating OldRating { get; } = OldRating;
    public Rating NewRating { get; } = NewRating;
}

public sealed class RatingRemovedEvent(Guid VisitedSiteId, Guid SiteId, Rating Rating) : BaseEvent
{
    public Guid VisitedSiteId { get; } = VisitedSiteId;
    public Guid SiteId { get; } = SiteId;
    public Rating Rating { get; } = Rating;
}

public sealed class TouristRegisteredEvent(string Email, string FirstName, string Token) : BaseEvent
{
    public string Email { get; } = Email;
    public string FirstName { get; } = FirstName;
    public string Token { get; } = Token;
}

public sealed class TouristFavoriteAddedEvent(Guid TouristId, Guid SiteId) : BaseEvent
{
    public Guid TouristId { get; } = TouristId;
    public Guid SiteId { get; } = SiteId;
}

public sealed class TouristFavoriteRemovedEvent(Guid TouristId, Guid SiteId) : BaseEvent
{
    public Guid TouristId { get; } = TouristId;
    public Guid SiteId { get; } = SiteId;
}