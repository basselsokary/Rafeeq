namespace Application.DTOs.Integrations.TripPlanner;

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
