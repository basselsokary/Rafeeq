using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.TripAggregate;

public class TripSite : BaseAuditableEntity
{
    public Guid SiteId { get; private set; }
    
    public string SiteName { get; private set; } = null!;
    public string SiteImageUrl { get; private set; } = null!;
    public SiteType SiteType { get; private set; }
    public string CityName { get; private set; } = null!;
    
    public GeoLocation SiteLocation { get; private set; } = null!;
    public Money EstimatedCost { get; private set; } = null!;
    public TimeOnly PlannedArrivalTime { get; private set; }
    public TimeSpan EstimatedDuration { get; private set; }
    
    public int VisitOrder { get; private set; }
    
    public bool IsVisited { get; private set; }
    public DateTime? ActualVisitTime { get; private set; }

    private TripSite() { }
    private TripSite(
        Guid siteId,
        string siteName,
        string siteImageUrl,
        SiteType siteType,
        string cityName,
        GeoLocation siteLocation,
        int visitOrder,
        TimeOnly plannedArrivalTime,
        TimeSpan estimatedDuration,
        Money estimatedCost)
    {
        SiteId = siteId;
        SiteName = siteName;
        SiteImageUrl = siteImageUrl;
        SiteType = siteType;
        CityName = cityName;
        SiteLocation = siteLocation;
        VisitOrder = visitOrder;
        EstimatedDuration = estimatedDuration;
        EstimatedCost = estimatedCost;
        PlannedArrivalTime = plannedArrivalTime;
        
        IsVisited = false;
    }
    
    internal static Result<TripSite> Create(
        Guid siteId,
        string siteName,
        string siteImageUrl,
        SiteType siteType,
        string cityName,
        GeoLocation siteLocation,
        int visitOrder,
        TimeOnly plannedArrivalTime,
        TimeSpan estimatedDuration,
        Money estimatedCost)
    {
        if (estimatedDuration.TotalMinutes <= 0)
            return TripErrors.EstimatedDurationInvalid;

        if (visitOrder < 0)
            return TripErrors.DisplayOrderInvalid;
        
        return new TripSite(
            siteId,
            siteName,
            siteImageUrl,
            siteType,
            cityName,
            siteLocation,
            visitOrder,
            plannedArrivalTime,
            estimatedDuration,
            estimatedCost);
    }
    
    public void MarkAsVisited(DateTime visitTime)
    {
        IsVisited = true;
        ActualVisitTime = visitTime;
    }
    
    public void UpdatePlannedTime(TimeOnly arrivalTime)
    {
        PlannedArrivalTime = arrivalTime;
    }

    internal Result UpdateVisitOrder(int visitOrder)
    {
        if (visitOrder < 0)
            return TripErrors.DisplayOrderInvalid;

        VisitOrder = visitOrder;
        return Result.Success();
    }
}
