using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Interfaces.Services;
using Application.DTOs.Trips;
using Microsoft.Extensions.Logging;
using Shared;

namespace Infrastructure.ExternalServices.TripPlannerService;

public class HttpTripPlannerService(
    HttpClient http,
    TripPlannerOptions settings,
    ILogger<HttpTripPlannerService> logger) : ITripPlannerService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<Result<TripPlanResponse>> PlanTripAsync(
        TripPlanRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var httpResponse = await http.PostAsJsonAsync(
                settings.PlanEndpoint, request, JsonOptions, ct);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync(ct);

                logger.LogWarning(
                    "Trip planner request failed. StatusCode: {StatusCode}, Response: {Response}",
                    httpResponse.StatusCode, errorContent);

                return TripPlannerErrors.TripPlannerFailed;
            }

            var response = await httpResponse.Content
                .ReadFromJsonAsync<TripPlanResponse>(JsonOptions, ct);

            if (response is null)
            {
                logger.LogError("Trip planner returned null response.");
                return TripPlannerErrors.TripPlannerFailed;
            }

            return Result.Success(response);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            logger.LogError("Trip planner request timed out.");
            return TripPlannerErrors.Timeout;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while calling trip planner service.");
            return Result.Failure<TripPlanResponse>(TripPlannerErrors.Unexpected);
        }
    }

    // private static TripPlanResponse MapToResult(TripPlanApiResponse response)
    // {
    //     return new TripPlanResult(
    //         Stops: response.Data.Stops.Select(s => new TripStopDto
    //         {
    //             Name = s.Name,
    //             ArrivalTime = s.ArrivalTime,
    //             PredictedDurationMinutes = s.PredictedDurationMinutes,
    //             TravelTimeMinutes = s.TravelTimeMinutes,
    //             TicketPrice = s.TicketPrice,
    //             Category = s.Category,
    //             Zone = s.Zone,
    //             Latitude = s.Latitude,
    //             Longitude = s.Longitude,
    //             Description = s.Description,
    //             FillPhase = s.FillPhase
    //         }).ToList(),

    //         Summary: new TripSummaryDto
    //         {
    //             TotalStops = response.Data.Summary.TotalStops,
    //             TotalTimeMinutes = response.Data.Summary.TotalTimeMinutes,
    //             TotalTicketCost = response.Data.Summary.TotalTicketCost,
    //             Currency = response.Data.Summary.Currency,
    //             BudgetLimit = response.Data.Summary.BudgetLimit,
    //             BudgetUsedPercentage = response.Data.Summary.BudgetUsedPercentage,
    //             BudgetStatus = response.Data.Summary.BudgetStatus,
    //             StartTime = response.Data.Summary.StartTime,
    //             EndTime = response.Data.Summary.EndTime
    //         });
    // }
}

public class TripPlannerErrors
{
    public static Error TripPlannerFailed =>
        Error.Failure("TRIP_PLANNER_FAILED", $"Trip planner service failed to generate a plan.");

    public static Error Timeout =>
        Error.Failure("TRIP_PLANNER_TIMEOUT", "Trip planner request timed out.");

    public static Error Unexpected =>
        Error.Failure("TRIP_PLANNER_UNEXPECTED", "Unexpected error while planning trip.");
}