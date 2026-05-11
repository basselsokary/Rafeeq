using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Application.DTOs.Sites;
using Application.DTOs.Trips;
using Domain.Common.Interfaces;
using Domain.Entities.TripAggregate;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

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
    ITripPlannerService tripPlannerService,
    ILogger<CreateTripCommand> logger) : ICommandHandler<CreateTripCommand, Guid>
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

        var visitedSites = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var visitedSitesOrdered = new List<string>();

        var remainingBudget = estimatedBudget;
        var daysCount = Math.Max(1, (command.EndDate.DayNumber - command.StartDate.DayNumber) + 1);
        var dayIndex = 0;

        for (var date = command.StartDate; date <= command.EndDate; date = date.AddDays(1))
        {
            dayIndex++;
            var remainingDays = Math.Max(1, daysCount - (dayIndex - 1));

            Money? dayBudget = null;
            if (remainingBudget != null)
            {
                var dayBudgetResult = remainingBudget.Divide(remainingDays);
                if (dayBudgetResult.Succeeded)
                    dayBudget = dayBudgetResult.Value;
            }

            var tripDayResult = trip.AddTripDay(date, dayBudget);
            if (tripDayResult.Failed)
                return tripDayResult.Error;

            var tripDay = tripDayResult.Value;

            var tripRequest = new TripPlanRequest(
                StartLat: userPositionResult.Value.Latitude,
                StartLon: userPositionResult.Value.Longitude,
                AvailableHours: availableHours,
                StartTime: command.DailyStartTime.ToString("HH:mm"),
                PreferredCategories: command.PreferredSiteTypes.Select(t => t.ToString()).ToList(),
                VisitedSites: visitedSitesOrdered,
                WalkingTolerance: (command.Tolerance ?? Tolerance.Low).ToString().ToLowerInvariant(),
                BudgetAmount: dayBudget?.Amount ?? 0,
                Currency: requestCurrency);

            var dailyTripResult = await tripPlannerService.PlanTripAsync(tripRequest, cancellationToken);
            if (dailyTripResult.Failed)
                return dailyTripResult.Error;
            
            logger.LogWarning("Received trip plan response for date {Date}: {@Response}", date, dailyTripResult.Value);

            var dailyTrip = dailyTripResult.Value;

            var itinerarySiteNames = dailyTrip.Itinerary
                .Select(s => s.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var sitesByName = new Dictionary<string, SiteListDto>(StringComparer.OrdinalIgnoreCase);
            if (itinerarySiteNames.Count > 0)
            {
                var itinerarySites = await siteQueryService.GetByNamesAsync(itinerarySiteNames, cancellationToken: cancellationToken);
                foreach (var site in itinerarySites)
                {
                    if (!sitesByName.ContainsKey(site.Name))
                        sitesByName[site.Name] = site;
                }
            }

            var actualCost = Money.Create((trip.ActualCost?.Amount ?? 0) + dailyTrip.TotalTicketCostEgp);
            trip.UpdateActualCost(actualCost.Succeeded ? actualCost.Value : null);

            var visitOrder = 0;
            foreach (var stop in dailyTrip.Itinerary)
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
                var cityName = siteFromDb?.CityName ?? (!string.IsNullOrWhiteSpace(stop.Zone) ? stop.Zone : dailyTrip.City);

                var siteLocation = userPositionResult.Value;
                if (siteFromDb != null)
                {
                    var siteLocationResult = GeoLocation.Create(siteFromDb.Location.Latitude, siteFromDb.Location.Longitude);
                    if (siteLocationResult.Succeeded)
                        siteLocation = siteLocationResult.Value;
                }

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

                if (visitedSites.Add(stop.Name))
                    visitedSitesOrdered.Add(stop.Name);
            }

            if (remainingBudget != null)
            {
                var spentEgp = Math.Max(0, dailyTrip.TotalTicketCostEgp);

                decimal spentInBudgetCurrency = 0;
                if (string.Equals(remainingBudget.Currency, "EGP", StringComparison.OrdinalIgnoreCase))
                {
                    spentInBudgetCurrency = spentEgp;
                }
                else
                {
                    if (dayBudget != null && dayBudget.Amount > 0 && dailyTrip.BudgetLimitEgp > 0)
                    {
                        var egpPerBudgetCurrencyUnit = dailyTrip.BudgetLimitEgp / dayBudget.Amount;
                        if (egpPerBudgetCurrencyUnit > 0)
                            spentInBudgetCurrency = spentEgp / egpPerBudgetCurrencyUnit;
                    }
                }

                var newRemainingAmount = Math.Max(0, remainingBudget.Amount - spentInBudgetCurrency);
                var newRemainingResult = Money.Create(newRemainingAmount, remainingBudget.Currency);
                remainingBudget = newRemainingResult.Succeeded ? newRemainingResult.Value : Money.Zero;
            }
        }

        // Here we print the full trip details with its sub-trips to the console for demonstration purposes
        Console.WriteLine();
        Console.WriteLine($"Trip ID: {trip.Id}");
        Console.WriteLine($"Name: {trip.Title}");
        Console.WriteLine($"Description: {trip.Description}");
        Console.WriteLine($"Date Range: {trip.StartDate:yyyy-MM-dd} to {trip.EndDate:yyyy-MM-dd}");
        Console.WriteLine($"User Position: Latitude {trip.UserPosition.Latitude}, Longitude {trip.UserPosition.Longitude}");
        Console.WriteLine($"Daily Time Range: {trip.DailyStartTime:HH\\:mm} to {trip.DailyEndTime:HH\\:mm}");
        // Console.WriteLine($"Preferred Site Types: {string.Join(", ", trip.PreferredSiteTypes)}");
        // Console.WriteLine($"Tolerance: {trip.Tolerance}");
        // Console.WriteLine("Trip Days:");
        // foreach (var day in trip.Days)
        // {
        //     Console.WriteLine($"\tDate: {day.Date:yyyy-MM-dd}");
        //     Console.WriteLine($"\tEstimated Daily Budget: {day.DayBudget?.Amount} {day.DayBudget?.Currency}");
        //     Console.WriteLine("\tSites:");
        //     foreach (var site in day.Sites)
        //     {
        //         Console.WriteLine($"\t\tName: {site.Name}");
        //         Console.WriteLine($"\t\tLocation: Latitude {site.Location.Latitude}, Longitude {site.Location.Longitude}");
        //         Console.WriteLine($"\t\tVisit Time: {site.VisitTimeRange}");
        //         Console.WriteLine($"\t\tTicket Price: {site.Cost.Amount} {site.Cost.Currency}");
        //         Console.WriteLine($"\t\tSite Type: {site.Type}");
        //         Console.WriteLine();
        //     }
        // }
        Console.WriteLine($"Estimated Budget: {trip.EstimatedTotalBudget?.ToString()}");
        Console.WriteLine($"Actual Cost: {trip.ActualCost?.ToString()}");

        await unitOfWork.Trips.AddAsync(trip, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return trip.Id;
    }
}
