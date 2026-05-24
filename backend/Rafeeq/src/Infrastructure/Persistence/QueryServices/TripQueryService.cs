using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Trips;
using Application.Extensions;
using Domain.Entities.TripAggregate;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class TripQueryService(
    ApplicationDbContext context) : ITripQueryService
{
    private IQueryable<Trip> Trips => context.Set<Trip>().AsNoTracking();

    public async Task<TripDetailDto?> GetByIdAsync(
        Guid id,
        Guid touristId,
        CancellationToken cancellationToken = default)
    {
        var data = await Trips
            .Where(t => t.Id == id && t.TouristId == touristId)
            .AsSplitQuery()
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.UserPosition,
                t.Status,
                t.Tolerance,
                t.EstimatedTotalBudget,
                t.ActualCost,
                t.TotalSites,
                t.EstimatedTotalDuration,
                t.CreatedAt,
                t.LastModifiedAt,
                PreferredSiteTypes = t.PreferredSiteTypes.ToList(),
                t.StartDate,
                t.EndDate,
                t.DailyStartTime,
                t.DailyEndTime,
                Days = t.Days
                    .OrderBy(d => d.DayNumber)
                    .Select(d => new
                    {
                        d.Id,
                        d.DayNumber,
                        d.Date,
                        d.EstimatedDayBudget,
                        d.TotalSites,
                        d.EstimatedDayDuration,
                        d.Notes,
                        Sites = d.Sites
                            .OrderBy(s => s.VisitOrder)
                            .Select(s => new
                            {
                                s.Id,
                                s.SiteName,
                                s.SiteImageUrl,
                                s.SiteType,
                                s.CityName,
                                s.SiteLocation,
                                s.EstimatedCost,
                                s.VisitOrder,
                                s.PlannedArrivalTime,
                                s.ActualVisitTime,
                                s.EstimatedDuration,
                                s.IsVisited
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (data is null)
            return null;

        var updatedAt = data.LastModifiedAt ?? data.CreatedAt;

        var days = data.Days
            .Select(d => new TripDayDto(
                d.Id,
                d.DayNumber,
                d.Date.ToDateTime(TimeOnly.MinValue),
                d.EstimatedDayBudget.ToDto(),
                d.TotalSites,
                (int)d.EstimatedDayDuration.TotalMinutes,
                d.Notes,
                d.Sites.Select(s => new TripSiteDto(
                    s.Id,
                    s.SiteName,
                    s.SiteImageUrl,
                    s.SiteType,
                    s.CityName,
                    s.SiteLocation.ToDto(),
                    s.EstimatedCost.ToDto(),
                    s.VisitOrder,
                    s.PlannedArrivalTime,
                    s.ActualVisitTime,
                    (int)s.EstimatedDuration.TotalMinutes,
                    s.IsVisited,
                    s.SiteType.ToString()
                )).ToList()
            ))
            .ToList();

        var preferredTypesDisplay = data.PreferredSiteTypes
            .Select(t => t.ToString())
            .ToList();

        return new TripDetailDto(
            data.Id,
            data.Title,
            data.Description,
            data.StartDate,
            data.EndDate,
            data.DailyStartTime,
            data.DailyEndTime,
            data.UserPosition.ToDto(),
            data.Status,
            data.Tolerance,
            data.EstimatedTotalBudget.ToDto(),
            data.ActualCost.ToDto(),
            data.TotalSites,
            (int)data.EstimatedTotalDuration.TotalMinutes,
            data.PreferredSiteTypes,
            days,
            data.CreatedAt,
            updatedAt,
            preferredTypesDisplay,
            StatusDisplay: data.Status.ToString(),
            ToleranceDisplay: data.Tolerance?.ToString() ?? string.Empty);
    }

    public async Task<PagedResult<TripListDto>> GetByTouristIdAsync(
        Guid touristId,
        PagingParameters paging,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = Trips
            .Where(t => t.TouristId == touristId)
            .OrderByDescending(t => t.StartDate)
            .ThenByDescending(t => t.CreatedAt);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Skip(paging.Skip)
            .Take(paging.Take)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.StartDate,
                t.EndDate,
                t.Status,
                TotalDays = t.Days.Count,
                t.TotalSites,
                t.EstimatedTotalDuration,
                t.EstimatedTotalBudget,
                t.ActualCost,
                t.CreatedAt,
                t.LastModifiedAt
            })
            .ToListAsync(cancellationToken);

        var dtos = items
            .Select(t => new TripListDto(
                t.Id,
                t.Title,
                t.Description ?? string.Empty,
                t.StartDate,
                t.EndDate,
                t.Status,
                t.TotalDays,
                t.TotalSites,
                (int)t.EstimatedTotalDuration.TotalMinutes,
                t.EstimatedTotalBudget.ToDto(),
                t.ActualCost.ToDto(),
                t.CreatedAt,
                t.LastModifiedAt ?? t.CreatedAt,
                StatusDisplay: t.Status.ToString()))
            .ToList();

        return new PagedResult<TripListDto>(
            dtos,
            totalCount,
            paging.Page,
            paging.PageSize);
    }
}
