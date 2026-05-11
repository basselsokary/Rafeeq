using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.Common.Interfaces;
using Shared;
using static Domain.Common.Constants.DomainConstants.Trip;

namespace Domain.Entities.TripAggregate;

public class Trip : BaseAuditableEntity, IAggregateRoot
{
    public Guid TouristId { get; private set; }
    
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public TimeOnly DailyStartTime { get; private set; }
    public TimeOnly DailyEndTime { get; private set; }
    
    public TripStatus Status { get; private set; }
    public GeoLocation UserPosition { get; private set; } = null!;
    
    public Tolerance? Tolerance { get; private set; }
    public Money? EstimatedTotalBudget { get; private set; }
    public Money? ActualCost { get; private set; }
    public TimeSpan EstimatedTotalDuration { get; private set; }
    
    public int TotalSites {get; private set; }

    private readonly List<SiteType> _preferredSiteTypes = [];
    public IReadOnlyCollection<SiteType> PreferredSiteTypes => _preferredSiteTypes.AsReadOnly();

    private readonly List<TripDay> _days = [];
    public IReadOnlyCollection<TripDay> Days => _days.AsReadOnly();

    private Trip() { }
    private Trip(
        Guid touristId,
        string title,
        string? description,
        GeoLocation userPosition,
        DateOnly startDate,
        DateOnly endDate,
        TimeOnly dailyStartTime,
        TimeOnly dailyEndTime,
        List<SiteType> siteTypes,
        Tolerance? tolerance,
        Money? estimatedBudget)
    {
        TouristId = touristId;
        Title = title;
        Description = description;
        UserPosition = userPosition;
        StartDate = startDate;
        EndDate = endDate;
        DailyStartTime = dailyStartTime;
        DailyEndTime = dailyEndTime;
        EstimatedTotalBudget = estimatedBudget;
        _preferredSiteTypes.AddRange(siteTypes);
        Tolerance = tolerance;
        
        Status = TripStatus.Draft;
        EstimatedTotalDuration = TimeSpan.Zero;
        TotalSites = 0;
    }

    public static Result<Trip> Create(
        Guid touristId,
        string title,
        string? description,
        GeoLocation userPosition,
        DateOnly startDate,
        DateOnly endDate,
        TimeOnly dailyStartTime,
        TimeOnly dailyEndTime,
        List<SiteType> siteTypes,
        Tolerance? tolerance,
        Money? estimatedBudget = null)
    {
        if (touristId == Guid.Empty)
            return TripErrors.TouristIdRequired;

        if (string.IsNullOrWhiteSpace(title))
            return TripErrors.TitleRequired;
        
        if (startDate > endDate)
            return DateRangeErrors.StartDateNotBeforeEndDate;

        var durationDays = (endDate.DayNumber - startDate.DayNumber) + 1;
        if (durationDays <= 0 || durationDays > MaxDurationDays)
            return TripErrors.InvalidDurationDays(MaxDurationDays);
        
        if (dailyStartTime >= dailyEndTime)
            return TimeRangeErrors.StartTimeNotBeforeEndTime;

        var trip = new Trip(
            touristId,
            title.Trim(),
            description?.Trim(),
            userPosition,
            startDate,
            endDate,
            dailyStartTime,
            dailyEndTime,
            siteTypes,
            tolerance,
            estimatedBudget);

        trip.RaiseDomainEvent(new TripCreatedEvent(trip.Id, trip.TouristId));

        return trip;
    }

    public Result UpdateBasicInfo(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            return TripErrors.TitleRequired;

        Title = title.Trim();
        Description = description?.Trim();
        RaiseDomainEvent(new TripUpdatedEvent(Id, TouristId));
        return Result.Success();
    }

    public Result UpdateActualCost(Money? actualCost)
    {
        ActualCost = actualCost;
        RaiseDomainEvent(new TripUpdatedEvent(Id, TouristId));
        return Result.Success();
    }

    public Result<TripDay> AddTripDay(DateOnly date, Money? dayBudget)
    {
        if (date < StartDate || date > EndDate)
            return TripErrors.TripDayDateOutOfRange;

        if (_days.Any(d => d.Date == date))
            return TripErrors.TripDayAlreadyExists;

        var dayNumber = date.DayNumber - StartDate.DayNumber + 1;
        var dayResult = TripDay.Create(dayNumber, date, dayBudget);
        if (dayResult.Failed)
            return dayResult.Error;

        _days.Add(dayResult.Value);
        RaiseDomainEvent(new TripUpdatedEvent(Id, TouristId));
        return dayResult.Value;
    }

    public Result AddSiteToDay(
        int dayNumber,
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
        var day = _days.FirstOrDefault(d => d.DayNumber == dayNumber);
        if (day == null)
            return TripErrors.TripDayNotFound;

        var site = TripSite.Create(
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
        
        if (site.Failed)
            return site;
        
        day.AddSite(site.Value);
        RecalculateEstimates();
        RaiseDomainEvent(new TripUpdatedEvent(Id, TouristId));
        return Result.Success();
    }
    
    public Result RemoveSiteFromDay(int dayNumber, Guid siteId)
    {
        var day = _days.FirstOrDefault(d => d.DayNumber == dayNumber);
        if (day == null)
            return TripErrors.TripDayNotFound;
        
        day.RemoveSite(siteId);
        RecalculateEstimates();
        RaiseDomainEvent(new TripUpdatedEvent(Id, TouristId));
        return Result.Success();
    }
    
    // public void MoveSiteToAnotherDay(Guid siteId, int fromDayNumber, int toDayNumber)
    // {
    //     var fromDay = _days.FirstOrDefault(d => d.DayNumber == fromDayNumber);
    //     var toDay = _days.FirstOrDefault(d => d.DayNumber == toDayNumber);
        
    //     if (fromDay == null || toDay == null)
    //         throw new BusinessRuleValidationException("Invalid day numbers");
        
    //     var site = fromDay.Sites.FirstOrDefault(s => s.Id == siteId);
    //     if (site == null)
    //         throw new EntityNotFoundException(nameof(TripSite), siteId);
        
    //     fromDay.RemoveSite(siteId);
    //     toDay.AddSite(site);
    //     RecalculateEstimates();
    //     MarkAsUpdated();
    // }
    
    // public void StartTrip()
    // {
    //     if (Status != TripStatus.Planning)
    //         throw new BusinessRuleValidationException("Can only start a trip that is in planning status");
        
    //     Status = TripStatus.InProgress;
    //     RaiseDomainEvent(new TripStartedEvent(Id, TouristId));
    //     MarkAsUpdated();
    // }
    
    // public void CompleteTrip()
    // {
    //     if (Status != TripStatus.InProgress)
    //         throw new BusinessRuleValidationException("Can only complete a trip that is in progress");
        
    //     Status = TripStatus.Completed;
    //     RaiseDomainEvent(new TripCompletedEvent(Id, TouristId));
    //     MarkAsUpdated();
    // }
    
    private void RecalculateEstimates()
    {
        EstimatedTotalDuration = TimeSpan.FromMinutes(_days.Sum(d => d.GetTotalDuration().TotalMinutes));
        TotalSites = _days.Sum(d => d.TotalSites);
    }

    public void Delete()
    {
        RaiseDomainEvent(new TripDeletedEvent(Id, TouristId));
    }
}