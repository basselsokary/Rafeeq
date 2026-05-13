namespace Application.DTOs.Trips;

public record TripPlanRequest(
    double StartLat,
    double StartLon,
    int Days,
    decimal TotalBudget = default,
    int AvailableHoursPerDay = default,
    string StartTime = default!,
    List<string> PreferredCategories = default!,
    string WalkingTolerance = default!,
    string Currency = default!);

public class TripStopDto
{
    public string Name { get; set; } = default!;
    public string ArrivalTime { get; set; } = default!;
    public double PredictedDurationMinutes { get; set; }
    public double TravelTimeMinutes { get; set; }
    public decimal TicketPriceEgp { get; set; }
    public string Category { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public sealed record TripPlanSummaryDto(
    int TotalDays,
    int TotalSitesVisited,
    decimal TotalTicketCostEgp,
    decimal TotalBudgetEgp,
    decimal DailyBudgetEgp,
    string Currency);

public sealed record TripPlanDayDto(
    int Day,
    string City,
    List<double> StartLocation,
    List<TripStopDto> Itinerary,
    decimal DayTicketCostEgp,
    decimal DayBudgetEgp,
    double TotalTimeMinutes,
    bool FallbackUsed,
    int FallbackLevel);

public sealed record TripPlanResponse(
    TripPlanSummaryDto TripSummary,
    List<TripPlanDayDto> Days);