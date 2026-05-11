namespace Application.DTOs.Trips;

public record TripPlanRequest(
    double StartLat,
    double StartLon,
    // string City = default!,
    int AvailableHours = default,
    string StartTime = default!,
    List<string> PreferredCategories = default!,
    List<string> VisitedSites = default!,
    string WalkingTolerance = default!,
    decimal BudgetAmount = default!,
    string Currency = default!);

// public class TripPlanApiResponse
// {
//     public string Status { get; set; } = default!;
//     public string System { get; set; } = default!;
//     public TripPlanData Data { get; set; } = default!;
// }

// public class TripPlanData
// {
//     public List<TripStopDto> Stops { get; set; } = [];
//     public TripSummaryDto Summary { get; set; } = default!;
// }

public class TripStopDto
{
    public string Name { get; set; } = default!;
    public string ArrivalTime { get; set; } = default!;
    public double PredictedDurationMinutes { get; set; }
    public double TravelTimeMinutes { get; set; }
    public decimal TicketPriceEgp { get; set; }
    public string Category { get; set; } = default!;
    public string Zone { get; set; } = default!;
}

// public class TripSummaryDto
// {
//     public int TotalStops { get; set; }
//     public int TotalTimeMinutes { get; set; }
//     public decimal TotalTicketCost { get; set; }
//     public string Currency { get; set; } = default!;
//     public decimal BudgetLimit { get; set; }
//     public double BudgetUsedPercentage { get; set; }
//     public string BudgetStatus { get; set; } = default!;
//     public string StartTime { get; set; } = default!;
//     public string EndTime { get; set; } = default!;
// }

public record TripPlanResponse(
    List<TripStopDto> Itinerary,
    string City,
    double TotalTimeMinutes,
    decimal TotalTicketCostEgp,
    decimal BudgetLimitEgp,
    string Currency);