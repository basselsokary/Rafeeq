using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Application.DTOs.Integrations.TripPlanner;
using Application.DTOs.Sites;
using Domain.Common.Interfaces;
using Domain.Entities.TripAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Trips;

public sealed record CreateTripCommand(
    string Title,
    string? Description,
    double Latitude,
    double Longitude,
    List<SiteType> PreferredSiteTypes,
    DateOnly StartDate,
    DateOnly EndDate,
    TimeOnly DailyStartTime,
    TimeOnly DailyEndTime,
    decimal? EstimatedBudget,
    string? Currency,
    Tolerance? Tolerance = null) : ICommand<Guid>;

internal sealed class CreateTripCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ISiteQueryService siteQueryService,
    ITripPlannerService tripPlannerService) : ICommandHandler<CreateTripCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateTripCommand command, CancellationToken cancellationToken)
    {
        var userPositionResult = GeoLocation.Create(command.Latitude, command.Longitude);
        if (userPositionResult.Failed)
            return userPositionResult.Error;

        var dailyTimeSpan = command.DailyEndTime.ToTimeSpan() - command.DailyStartTime.ToTimeSpan();
        var availableHours = (int)dailyTimeSpan.TotalHours;

        var requestCurrency = (command.Currency ?? "EGP").ToUpperInvariant();

        Money? estimatedBudget = null;
        if (command.EstimatedBudget.HasValue)
        {
            var estimatedBudgetResult = Money.Create(command.EstimatedBudget.Value, requestCurrency);
            if (estimatedBudgetResult.Failed)
                return estimatedBudgetResult.Error;

            estimatedBudget = estimatedBudgetResult.Value;
        }

        var tripResult = Trip.Create(
            userContext.Id,
            command.Title,
            command.Description,
            userPositionResult.Value,
            command.StartDate,
            command.EndDate,
            command.DailyStartTime,
            command.DailyEndTime,
            command.PreferredSiteTypes,
            command.Tolerance,
            estimatedBudget);

        if (tripResult.Failed)
            return tripResult.Error;

        var trip = tripResult.Value;

        var daysCount = Math.Max(1, command.EndDate.DayNumber - command.StartDate.DayNumber + 1);
        var tripRequest = BuildTripRequest(command, userPositionResult.Value, daysCount, availableHours, estimatedBudget, requestCurrency);

        var tripPlanResult = await tripPlannerService.PlanTripAsync(tripRequest, cancellationToken);
        if (tripPlanResult.Failed)
            return tripPlanResult.Error;

        var tripPlan = tripPlanResult.Value;

        var sitesByName = await LoadSitesByNameAsync(tripPlan, cancellationToken);

        var dayIndex = 0;
        foreach (var planDay in tripPlan.Days)
        {
            dayIndex++;
            var dayNumber = planDay.Day > 0 ? planDay.Day : dayIndex;
            var date = command.StartDate.AddDays(dayNumber - 1);
            var dayBudget = BuildDayBudget(command.EstimatedBudget, planDay.DayBudgetEgp ?? 0);

            var tripDayResult = trip.AddTripDay(date, dayBudget);
            if (tripDayResult.Failed)
                return tripDayResult.Error;

            var tripDay = tripDayResult.Value;

            var visitOrder = 0;
            foreach (var stop in planDay.Itinerary)
            {
                var arrivalTime = TimeOnly.ParseExact(stop.ArrivalTime, @"HH\:mm");

                var estimatedDurationMinutes = Math.Max(1, (int)Math.Ceiling(stop.PredictedDurationMinutes));
                var estimatedDuration = TimeSpan.FromMinutes(estimatedDurationMinutes);

                var costResult = Money.Create(stop.TicketPriceEgp, "EGP");
                if (costResult.Failed)
                    return costResult.Error;

                sitesByName.TryGetValue(stop.Name, out var siteFromDb);

                var siteId = siteFromDb?.Id ?? Guid.NewGuid();
                var siteName = siteFromDb?.Name ?? stop.Name;
                var siteImageUrl = siteFromDb?.PrimaryImageUrl ?? string.Empty;
                var siteType = siteFromDb?.Type ?? Enum.Parse<SiteType>(stop.Category, true);
                var cityName = siteFromDb?.CityName ?? (!string.IsNullOrWhiteSpace(stop.Zone) ? stop.Zone : planDay.City);
                var siteLocation = ResolveSiteLocation(userPositionResult.Value, siteFromDb, stop);

                var addSiteResult = trip.AddSiteToDay(
                    dayNumber: tripDay.DayNumber,
                    siteId: siteId,
                    siteName: siteName,
                    siteImageUrl: siteImageUrl,
                    siteType: siteType,
                    cityName: cityName,
                    siteLocation: siteLocation,
                    visitOrder: visitOrder++,
                    plannedArrivalTime: arrivalTime,
                    estimatedDuration: estimatedDuration,
                    estimatedCost: costResult.Value);

                if (addSiteResult.Failed)
                    return addSiteResult.Error;
            }
        }

        var actualCostResult = Money.Create(tripPlan.TripSummary.TotalTicketCostEgp, "EGP");
        trip.UpdateActualCost(actualCostResult.Succeeded ? actualCostResult.Value : null);

        var tourist = await unitOfWork.Tourists.GetByIdAsync(userContext.Id, cancellationToken);
        tourist?.IncrementTripCount();

        await unitOfWork.Trips.AddAsync(trip, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return trip.Id;
    }

    private static TripPlanRequest BuildTripRequest(
        CreateTripCommand command,
        GeoLocation userPosition,
        int daysCount,
        int availableHours,
        Money? estimatedBudget,
        string requestCurrency)
        => new(
            StartLat: userPosition.Latitude,
            StartLon: userPosition.Longitude,
            Days: daysCount,
            TotalBudget: estimatedBudget?.Amount ?? 0,
            AvailableHoursPerDay: availableHours,
            StartTime: command.DailyStartTime.ToString("HH:mm"),
            PreferredCategories: command.PreferredSiteTypes.Select(t => t.ToString()).ToList(),
            WalkingTolerance: (command.Tolerance ?? Tolerance.Low).ToString().ToLowerInvariant(),
            Currency: requestCurrency);

    private async Task<Dictionary<string, SiteListDto>> LoadSitesByNameAsync(
        TripPlanResponse tripPlan,
        CancellationToken cancellationToken)
    {
        var itinerarySiteNames = tripPlan.Days
            .SelectMany(day => day.Itinerary)
            .Select(stop => stop.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var sitesByName = new Dictionary<string, SiteListDto>(StringComparer.OrdinalIgnoreCase);
        if (itinerarySiteNames.Count == 0)
            return sitesByName;

        var itinerarySites = await siteQueryService.GetByEnglishNamesAsync(
            itinerarySiteNames,
            language: userContext.Language,
            cancellationToken: cancellationToken);

        foreach (var site in itinerarySites)
        {
            if (!sitesByName.ContainsKey(site.Key))
                sitesByName[site.Key] = site.Value;
        }

        return sitesByName;
    }

    private static Money? BuildDayBudget(decimal? estimatedBudget, decimal dayBudgetEgp)
    {
        if (!estimatedBudget.HasValue || dayBudgetEgp <= 0)
            return null;

        var dayBudgetResult = Money.Create(dayBudgetEgp, "EGP");
        return dayBudgetResult.Succeeded ? dayBudgetResult.Value : null;
    }

    private static GeoLocation ResolveSiteLocation(
        GeoLocation fallback,
        SiteListDto? siteFromDb,
        TripStopDto stop)
    {
        if (siteFromDb != null)
        {
            var siteLocationResult = GeoLocation.Create(siteFromDb.Location.Latitude, siteFromDb.Location.Longitude);
            if (siteLocationResult.Succeeded)
                return siteLocationResult.Value;
        }

        if (stop.Latitude.HasValue && stop.Longitude.HasValue)
        {
            var stopLocationResult = GeoLocation.Create(stop.Latitude.Value, stop.Longitude.Value);
            if (stopLocationResult.Succeeded)
                return stopLocationResult.Value;
        }

        return fallback;
    }
}
