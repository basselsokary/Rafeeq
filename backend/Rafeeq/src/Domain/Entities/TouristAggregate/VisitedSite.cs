using Domain.Common;
using Domain.Events;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.TouristAggregate;

public class VisitedSite : BaseAuditableEntity
{
    public Guid SiteId { get; private set; }
    public Guid TouristId { get; private set; }

    public DateTime VisitDate { get; private set; }
    public int DurationMinutes { get; private set; }
    public Rating? Rating { get; private set; }
    public string? Notes { get; private set; }

    private VisitedSite() { }
    private VisitedSite(Guid siteId, int durationMinutes, DateTime visitDate)
    {
        SiteId = siteId;
        VisitDate = visitDate;
        DurationMinutes = durationMinutes;
    }

    internal static Result<VisitedSite> Create(Guid siteId, int durationMinutes, DateTime? visitDate)
    {
        if (visitDate > DateTime.UtcNow)
            return TouristErrors.VisitDateInFuture;

        if (durationMinutes <= 0)
            return TouristErrors.DurationInvalid;

        return new VisitedSite(siteId, durationMinutes, visitDate ?? DateTime.UtcNow);
    }

    public void SetRating(Rating rating)
    {
        Rating = rating;

        RaiseDomainEvent(new RatingAddedEvent(Id, SiteId, TouristId, Rating));
    }

    public void UpdateRating(Rating rating)
    {
        if (Rating is null)
        {
            SetRating(rating);
            return;
        }

        var oldRating = Rating;

        Rating = rating;

        RaiseDomainEvent(new RatingUpdatedEvent(Id, SiteId, oldRating, Rating));
    }

    public void RemoveRating()
    {
        if (Rating is not null)
        {
            RaiseDomainEvent(new RatingRemovedEvent(Id, SiteId, Rating));
            Rating = null;
        }
    }

    public void AddNotes(string notes)
    {
        Notes = notes?.Trim();
    }

    public Result UpdateDuration(int durationMinutes)
    {
        if (durationMinutes <= 0)
            return TouristErrors.DurationInvalid;

        DurationMinutes = durationMinutes;
        return Result.Success();
    }

    public bool IsRated() => Rating is not null;
}
