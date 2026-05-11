using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using Shared;

namespace Domain.Entities.TripAggregate;

public class TripDay : BaseAuditableEntity
{
    public int DayNumber { get; private set; }
    public DateOnly Date { get; private set; }

    public Money? EstimatedDayBudget { get; private set; }
    public TimeSpan EstimatedDayDuration { get; private set; }

    public int TotalSites {get; private set; }
    public string? Notes { get; private set; }
    
    private readonly List<TripSite> _sites = [];
    public IReadOnlyCollection<TripSite> Sites => _sites.AsReadOnly();

    private TripDay() { }
    private TripDay(int dayNumber, DateOnly date, Money? dayBudget)
    {
        DayNumber = dayNumber;
        Date = date;
        EstimatedDayBudget = dayBudget;

        TotalSites = 0;
        EstimatedDayDuration = TimeSpan.Zero;
    }

    public static Result<TripDay> Create(DateOnly date, Money? dayBudget)
    {
        var tripDay = new TripDay(0, date, dayBudget);
        return Result.Success(tripDay);
    }

    internal static Result<TripDay> Create(int dayNumber, DateOnly date, Money? dayBudget)
    {
        if (dayNumber <= 0)
            return TripErrors.DayNumberInvalid;

        var tripDay = new TripDay(dayNumber, date, dayBudget);
        return Result.Success(tripDay);
    }

    public Result AddSite(TripSite tripSite)
    {
        if (_sites.Any(s => s.SiteId == tripSite.SiteId))
            return TripErrors.SiteAlreadyAdded;

        _sites.Add(tripSite);
        RecalculateTotalDuration();
        TotalSites++;
        
        return Result.Success();
    }

    public Result RemoveSite(Guid siteId)
    {
        var site = _sites.FirstOrDefault(a => a.SiteId == siteId);
        if (site == null)
            return TripErrors.SiteNotFound;

        _sites.Remove(site);
        
        RecalculateTotalDuration();
        ReorderSites();
        TotalSites--;

        return Result.Success();
    }

    // public Result UpdateSiteOrder(Guid tripSiteId, int newOrder)
    // {
    //     var site = _sites.FirstOrDefault(a => a.Id == tripSiteId);
    //     if (site == null)
    //         return TripErrors.SiteNotFound;

    //     site.UpdateDisplayOrder(newOrder);
    //     ReorderSites();
    //     return Result.Success();
    // }

    // public Result MarkSiteAsVisited(Guid tripSiteId)
    // {
    //     var site = _sites.FirstOrDefault(a => a.Id == tripSiteId);
    //     if (site == null)
    //         return TripErrors.SiteAlreadyMarkedAsVisited;

    //     site.MarkAsVisited();
    //     return Result.Success();
    // }

    public int GetCompletionPercentage()
    {
        if (_sites.Count == 0) return 0;
        var visitedCount = _sites.Count(a => a.IsVisited);
        return (int)((double)visitedCount / _sites.Count * 100);
    }

    public TimeSpan GetTotalDuration()
    {
        return TimeSpan.FromMinutes(_sites.Sum(s => s.EstimatedDuration.TotalMinutes));
    }
    
    public decimal GetTotalCost()
    {
        return _sites.Sum(s => s.EstimatedCost.Amount);
    }

    private void RecalculateTotalDuration()
    {
        EstimatedDayDuration = TimeSpan.FromMinutes(_sites.Sum(a => a.EstimatedDuration.TotalMinutes));
    }

    private void ReorderSites()
    {
        var ordered = _sites.OrderBy(a => a.ActualVisitTime).ThenBy(a => a.VisitOrder).ToList();
        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].UpdateVisitOrder(i);
        }
    }

    public void AddNotes(string notes)
    {
        Notes = notes?.Trim();
    }
}