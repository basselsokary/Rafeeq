using Domain.Common;
using Domain.Common.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.TripAggregate;

public class TripAttraction : BaseAuditableEntity
{
    public Guid AttractionId { get; private set; }
    public DateTime VisitDate { get; private set; }
    public TimeRange? VisitTimeRange { get; private set; }
    public int EstimatedDurationMinutes { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsVisited { get; private set; }
    public DateTime? ActualVisitTime { get; private set; }
    public int? ActualDurationMinutes { get; private set; }
    public string? Notes { get; private set; }

    private TripAttraction() { }
    private TripAttraction(
        Guid attractionId,
        DateTime visitDate,
        TimeRange? visitTimeRange,
        int estimatedDurationMinutes,
        int displayOrder)
    {
        AttractionId = attractionId;
        VisitDate = visitDate.Date;
        VisitTimeRange = visitTimeRange;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        DisplayOrder = displayOrder;
        
        IsVisited = false;
    }

    internal static TripAttraction Create(
        Guid attractionId,
        DateTime visitDate,
        TimeRange? visitTimeRange,
        int estimatedDurationMinutes,
        int displayOrder)
    {
        if (estimatedDurationMinutes <= 0)
            throw new BusinessRuleValidationException("Estimated duration must be greater than zero.");

        if (displayOrder < 0)
            throw new BusinessRuleValidationException("Display order cannot be negative.");

        return new TripAttraction(
            attractionId,
            visitDate,
            visitTimeRange,
            estimatedDurationMinutes,
            displayOrder);
    }

    internal void UpdateDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new BusinessRuleValidationException("Display order cannot be negative.");

        DisplayOrder = displayOrder;
        MarkAsUpdated();
    }

    internal void MarkAsVisited(int actualDurationMinutes)
    {
        if (actualDurationMinutes <= 0)
            throw new BusinessRuleValidationException("Actual duration must be greater than zero.");

        IsVisited = true;
        ActualVisitTime = DateTime.UtcNow;
        ActualDurationMinutes = actualDurationMinutes;
        MarkAsUpdated();
    }

    public void UpdateVisitSchedule(DateTime visitDate, TimeRange? visitTimeRange)
    {
        VisitDate = visitDate.Date;
        VisitTimeRange = visitTimeRange;
        MarkAsUpdated();
    }

    public void UpdateEstimatedDuration(int estimatedDurationMinutes)
    {
        if (estimatedDurationMinutes <= 0)
            throw new BusinessRuleValidationException("Estimated duration must be greater than zero.");

        EstimatedDurationMinutes = estimatedDurationMinutes;
        MarkAsUpdated();
    }

    public void AddNotes(string notes)
    {
        Notes = notes?.Trim();
        MarkAsUpdated();
    }
}
