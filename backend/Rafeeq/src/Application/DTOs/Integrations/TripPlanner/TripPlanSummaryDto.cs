namespace Application.DTOs.Integrations.TripPlanner;

public sealed record TripPlanSummaryDto(
    int TotalDays,
    int TotalSitesVisited,
    decimal TotalTicketCostEgp,
    decimal? TotalBudgetEgp,
    decimal? DailyBudgetEgp,
    string Currency);
