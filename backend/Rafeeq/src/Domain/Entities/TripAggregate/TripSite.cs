using Domain.Common;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.TripAggregate;

public class TripSite : BaseAuditableEntity
{
    public Guid SiteId { get; private set; }
    public DateTime VisitDate { get; private set; }
    public TimeRange? VisitTimeRange { get; private set; }
    public int EstimatedDurationMinutes { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsVisited { get; private set; }
    public DateTime? ActualVisitTime { get; private set; }
    public int? ActualDurationMinutes { get; private set; }
    public string? Notes { get; private set; }

    private TripSite() { }
    private TripSite(
        Guid siteId,
        DateTime visitDate,
        TimeRange? visitTimeRange,
        int estimatedDurationMinutes,
        int displayOrder)
    {
        SiteId = siteId;
        VisitDate = visitDate.Date;
        VisitTimeRange = visitTimeRange;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        DisplayOrder = displayOrder;
        
        IsVisited = false;
    }

    internal static TripSite Create(
        Guid siteId,
        DateTime visitDate,
        TimeRange? visitTimeRange,
        int estimatedDurationMinutes,
        int displayOrder)
    {
        if (estimatedDurationMinutes <= 0)
            throw new BusinessRuleValidationException("Estimated duration must be greater than zero.");

        if (displayOrder < 0)
            throw new BusinessRuleValidationException("Display order cannot be negative.");

        return new TripSite(
            siteId,
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
