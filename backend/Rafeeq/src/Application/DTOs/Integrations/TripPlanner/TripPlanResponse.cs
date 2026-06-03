namespace Application.DTOs.Integrations.TripPlanner;

public sealed record TripPlanResponse(
    TripPlanSummaryDto TripSummary,
    List<TripPlanDayDto> Days);