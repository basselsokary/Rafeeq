using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.Common.Interfaces;

namespace Domain.Entities.TripAggregate;

public class Trip : BaseAuditableEntity, IAggregateRoot
{
    public Guid TouristId { get; private set; }
    
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateRange DateRange { get; private set; } = null!;
    public TripStatus Status { get; private set; }
    public TransportationType PreferredTransportation { get; private set; }
    public int TotalAttractions => _attractions.Count;
    public int EstimatedTotalDurationMinutes { get; private set; }
    public int ShareCount { get; private set; }

    private readonly List<TripAttraction> _attractions = [];
    public IReadOnlyCollection<TripAttraction> Attractions => _attractions.AsReadOnly();

    private Trip() { }
    private Trip(
        Guid touristId,
        string name,
        DateRange dateRange,
        TransportationType preferredTransportation)
    {
        TouristId = touristId;
        Name = name;
        DateRange = dateRange;
        Status = TripStatus.Draft;
        PreferredTransportation = preferredTransportation;
        EstimatedTotalDurationMinutes = 0;
        ShareCount = 0;
    }

    public static Trip Create(
        Guid touristId,
        string name,
        DateRange dateRange,
        TransportationType preferredTransportation = TransportationType.Walking)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Trip name cannot be empty.");

        var trip = new Trip(
            touristId,
            name.Trim(),
            dateRange,
            preferredTransportation);

        // trip.RaiseDomainEvent(new TripCreatedEvent(trip.Id, trip.TouristId, trip.Name));

        return trip;
    }

    public void UpdateBasicInfo(string name, string? description, TransportationType preferredTransportation)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Trip name cannot be empty.");

        Name = name.Trim();
        Description = description?.Trim();
        PreferredTransportation = preferredTransportation;
        MarkAsUpdated();
    }

    public void UpdateDateRange(DateRange dateRange)
    {
        DateRange = dateRange;
        MarkAsUpdated();
    }

    public void AddAttraction(
        Guid attractionId,
        DateTime visitDate,
        TimeRange? visitTimeRange,
        int estimatedDurationMinutes,
        int displayOrder)
    {
        if (!DateRange.IsWithinRange(visitDate))
            throw new BusinessRuleValidationException("Visit date must be within trip date range.");

        if (_attractions.Any(a => a.AttractionId == attractionId))
            throw new BusinessRuleValidationException("Attraction is already in the trip.");

        if (estimatedDurationMinutes <= 0)
            throw new BusinessRuleValidationException("Estimated duration must be greater than zero.");

        var tripAttraction = TripAttraction.Create(
            attractionId,
            visitDate,
            visitTimeRange,
            estimatedDurationMinutes,
            displayOrder);

        _attractions.Add(tripAttraction);
        RecalculateTotalDuration();
        MarkAsUpdated();
    }

    public void RemoveAttraction(Guid tripAttractionId)
    {
        var attraction = _attractions.FirstOrDefault(a => a.Id == tripAttractionId)
            ?? throw new EntityNotFoundException(nameof(TripAttraction), tripAttractionId);

        _attractions.Remove(attraction);
        RecalculateTotalDuration();
        ReorderAttractions();
        MarkAsUpdated();
    }

    public void UpdateAttractionOrder(Guid tripAttractionId, int newOrder)
    {
        var attraction = _attractions.FirstOrDefault(a => a.Id == tripAttractionId)
            ?? throw new EntityNotFoundException(nameof(TripAttraction), tripAttractionId);

        attraction.UpdateDisplayOrder(newOrder);
        ReorderAttractions();
        MarkAsUpdated();
    }

    public void MarkAttractionAsVisited(Guid tripAttractionId, int actualDurationMinutes)
    {
        var attraction = _attractions.FirstOrDefault(a => a.Id == tripAttractionId)
            ?? throw new EntityNotFoundException(nameof(TripAttraction), tripAttractionId);

        attraction.MarkAsVisited(actualDurationMinutes);
        CheckIfTripCompleted();
        MarkAsUpdated();
    }

    public void StartTrip()
    {
        if (Status != TripStatus.Planned)
            throw new InvalidOperationDomainException("Only planned trips can be started.");

        if (DateRange.StartDate > DateTime.UtcNow.Date)
            throw new InvalidOperationDomainException("Cannot start a trip before its start date.");

        UpdateStatus(TripStatus.InProgress);
    }

    public void CompleteTrip()
    {
        if (Status != TripStatus.InProgress)
            throw new InvalidOperationDomainException("Only in-progress trips can be completed.");

        UpdateStatus(TripStatus.Completed);
    }

    public void CancelTrip()
    {
        if (Status == TripStatus.Completed)
            throw new InvalidOperationDomainException("Cannot cancel a completed trip.");

        UpdateStatus(TripStatus.Cancelled);
    }

    public void PublishTrip()
    {
        if (Status == TripStatus.Draft)
            throw new InvalidOperationDomainException("Cannot publish a draft trip.");

        UpdateStatus(TripStatus.Planned);
    }

    public int GetCompletionPercentage()
    {
        if (_attractions.Count == 0) return 0;
        var visitedCount = _attractions.Count(a => a.IsVisited);
        return (int)((double)visitedCount / _attractions.Count * 100);
    }

    private void UpdateStatus(TripStatus newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;
        MarkAsUpdated();
        // RaiseDomainEvent(new TripStatusChangedEvent(Id, newStatus));
    }

    private void RecalculateTotalDuration()
    {
        EstimatedTotalDurationMinutes = _attractions.Sum(a => a.EstimatedDurationMinutes);
    }

    private void ReorderAttractions()
    {
        var ordered = _attractions.OrderBy(a => a.VisitDate).ThenBy(a => a.DisplayOrder).ToList();
        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].UpdateDisplayOrder(i);
        }
    }

    private void CheckIfTripCompleted()
    {
        if (Status == TripStatus.InProgress && _attractions.All(a => a.IsVisited))
        {
            UpdateStatus(TripStatus.Completed);
        }
    }
}
