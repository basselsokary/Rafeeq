namespace Application.DTOs.Integrations.TripPlanner;

public sealed record TripPlanDayDto(
    int Day,
    string City,
    List<double> StartLocation,
    List<TripStopDto> Itinerary,
    decimal DayTicketCostEgp,
    decimal? DayBudgetEgp,
    double TotalTimeMinutes,
    bool FallbackUsed,
    int FallbackLevel);
