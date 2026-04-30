using Domain.Common;
using Domain.Entities.TouristAggregate;
using Domain.ValueObjects;
using Shared;

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

    internal static Result<TripSite> Create(
        Guid siteId,
        DateTime visitDate,
        TimeRange? visitTimeRange,
        int estimatedDurationMinutes,
        int displayOrder)
    {
        if (estimatedDurationMinutes <= 0)
            return TouristErrors.DurationInvalid;

        if (displayOrder < 0)
            return TouristErrors.DisplayOrderInvalid;

        return new TripSite(
            siteId,
            visitDate,
            visitTimeRange,
            estimatedDurationMinutes,
            displayOrder);
    }

    internal Result UpdateDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            return TouristErrors.DisplayOrderInvalid;

        DisplayOrder = displayOrder;
        return Result.Success();
    }

    internal Result MarkAsVisited(int actualDurationMinutes)
    {
        if (actualDurationMinutes <= 0)
            return TouristErrors.DurationInvalid;

        IsVisited = true;
        ActualVisitTime = DateTime.UtcNow;
        ActualDurationMinutes = actualDurationMinutes;
        return Result.Success();
    }

    public void UpdateVisitSchedule(DateTime visitDate, TimeRange? visitTimeRange)
    {
        VisitDate = visitDate.Date;
        VisitTimeRange = visitTimeRange;
    }

    public Result UpdateEstimatedDuration(int estimatedDurationMinutes)
    {
        if (estimatedDurationMinutes <= 0)
            return TouristErrors.DurationInvalid;

        EstimatedDurationMinutes = estimatedDurationMinutes;
        return Result.Success();
    }

    public void AddNotes(string notes)
    {
        Notes = notes?.Trim();
    }
}
