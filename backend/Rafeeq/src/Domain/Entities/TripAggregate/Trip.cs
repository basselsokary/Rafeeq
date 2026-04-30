using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.Common.Interfaces;
using Shared;

namespace Domain.Entities.TripAggregate;

public class Trip : BaseAuditableEntity, IAggregateRoot
{
    public Guid TouristId { get; private set; }
    
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateRange DateRange { get; private set; } = null!;
    public TripStatus Status { get; private set; }
    public TransportationType PreferredTransportation { get; private set; }
    
    public Money? EstimatedBudget { get; private set; }
    public Money? ActualCost { get; private set; }

    public int TotalSites => _sites.Count;
    public int EstimatedTotalDurationMinutes { get; private set; }
    public int ShareCount { get; private set; }
    public bool IsPublic { get; private set; }

    private readonly List<TripSite> _sites = [];
    public IReadOnlyCollection<TripSite> Sites => _sites.AsReadOnly();

    private readonly List<TripNote> _notes = [];
    public IReadOnlyCollection<TripNote> Notes => _notes.AsReadOnly();

    private Trip() { }
    private Trip(
        Guid touristId,
        string name,
        DateRange dateRange,
        TransportationType preferredTransportation,
        Money? estimatedBudget)
    {
        TouristId = touristId;
        Name = name;
        DateRange = dateRange;
        PreferredTransportation = preferredTransportation;
        EstimatedBudget = estimatedBudget;
        
        Status = TripStatus.Draft;
        EstimatedTotalDurationMinutes = 0;
        ShareCount = 0;
        IsPublic = false;
    }

    public static Result<Trip> Create(
        Guid touristId,
        string name,
        DateRange dateRange,
        TransportationType preferredTransportation,
        Money? estimatedBudget = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return TripErrors.NameRequired;

        var trip = new Trip(
            touristId,
            name.Trim(),
            dateRange,
            preferredTransportation,
            estimatedBudget);

        // trip.RaiseDomainEvent(new TripCreatedEvent(trip.Id, trip.TouristId, trip.Name));

        return trip;
    }

    public Result UpdateBasicInfo(string name, string? description, TransportationType preferredTransportation)
    {
        if (string.IsNullOrWhiteSpace(name))
            return TripErrors.NameRequired;

        Name = name.Trim();
        Description = description?.Trim();
        PreferredTransportation = preferredTransportation;
        return Result.Success();
    }

    public void UpdateDateRange(DateRange dateRange)
    {
        DateRange = dateRange;
    }

    public Result AddSite(
        Guid siteId,
        DateTime visitDate,
        TimeRange? visitTimeRange,
        int estimatedDurationMinutes,
        int displayOrder)
    {
        if (!DateRange.IsWithinRange(visitDate))
            return TripErrors.SiteVisitDateOutOfRange;

        if (_sites.Any(a => a.SiteId == siteId))
            return TripErrors.SiteAlreadyAdded;

        if (estimatedDurationMinutes <= 0)
            return TripErrors.EstimatedDurationInvalid;

        var tripSite = TripSite.Create(
            siteId,
            visitDate,
            visitTimeRange,
            estimatedDurationMinutes,
            displayOrder);

        if (tripSite.Failed)
            return tripSite;

        _sites.Add(tripSite.Value);
        RecalculateTotalDuration();
        return Result.Success();
    }

    public Result RemoveSite(Guid tripSiteId)
    {
        var site = _sites.FirstOrDefault(a => a.Id == tripSiteId);
        if (site == null)
            return TripErrors.SiteNotFound;

        _sites.Remove(site);
        RecalculateTotalDuration();
        ReorderSites();
        return Result.Success();
    }

    public Result AddNote(string content, string? title = null)
    {
        var noteResult = TripNote.Create(content, title);
        if (noteResult.Failed)
            return noteResult;
    
        _notes.Add(noteResult.Value);
        return Result.Success();
    }

    public Result UpdateNote(Guid noteId, string content, string? title = null)
    {
        var note = _notes.FirstOrDefault(n => n.Id == noteId);
        if (note == null)
            return TripErrors.NoteNotFound;

        note.Update(content, title);
        return Result.Success();
    }

    public void RemoveNote(Guid noteId)
    {
        var note = _notes.FirstOrDefault(n => n.Id == noteId);
        if (note == null)
            return;

        _notes.Remove(note);
    }

    public Result UpdateSiteOrder(Guid tripSiteId, int newOrder)
    {
        var site = _sites.FirstOrDefault(a => a.Id == tripSiteId);
        if (site == null)
            return TripErrors.SiteNotFound;

        site.UpdateDisplayOrder(newOrder);
        ReorderSites();
        return Result.Success();
    }

    public Result MarkSiteAsVisited(Guid tripSiteId, int actualDurationMinutes)
    {
        var site = _sites.FirstOrDefault(a => a.Id == tripSiteId);
        if (site == null)
            return TripErrors.SiteAlreadyMarkedAsVisited;

        site.MarkAsVisited(actualDurationMinutes);
        CheckIfTripCompleted();
        return Result.Success();
    }

    public Result StartTrip()
    {
        if (Status != TripStatus.Planned)
            return TripErrors.CannotBeStarted;

        if (DateRange.StartDate > DateTime.UtcNow.Date)
            return TripErrors.CannotBeStarted;

        UpdateStatus(TripStatus.InProgress);
        return Result.Success();
    }

    public Result CompleteTrip()
    {
        if (Status != TripStatus.InProgress)
            return TripErrors.CannotBeCompleted;

        UpdateStatus(TripStatus.Completed);
        return Result.Success();
    }

    public Result CancelTrip()
    {
        if (Status == TripStatus.Completed)
            return TripErrors.CannotBeCanceled;

        UpdateStatus(TripStatus.Cancelled);
        return Result.Success();
    }

    public Result PublishTrip()
    {
        if (Status == TripStatus.Draft)
            return TripErrors.CannotBePublished;

        UpdateStatus(TripStatus.Planned);
        return Result.Success();
    }

    public void SetVisibility(bool isPublic)
    {
        IsPublic = isPublic;
    }

    public void IncrementShareCount()
    {
        ShareCount++;
    }
    
    public int GetCompletionPercentage()
    {
        if (_sites.Count == 0) return 0;
        var visitedCount = _sites.Count(a => a.IsVisited);
        return (int)((double)visitedCount / _sites.Count * 100);
    }

    private void UpdateStatus(TripStatus newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;
        // RaiseDomainEvent(new TripStatusChangedEvent(Id, newStatus));
    }

    private void RecalculateTotalDuration()
    {
        EstimatedTotalDurationMinutes = _sites.Sum(a => a.EstimatedDurationMinutes);
    }

    private void ReorderSites()
    {
        var ordered = _sites.OrderBy(a => a.VisitDate).ThenBy(a => a.DisplayOrder).ToList();
        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].UpdateDisplayOrder(i);
        }
    }

    private void CheckIfTripCompleted()
    {
        if (Status == TripStatus.InProgress && _sites.All(a => a.IsVisited))
        {
            UpdateStatus(TripStatus.Completed);
        }
    }
}
