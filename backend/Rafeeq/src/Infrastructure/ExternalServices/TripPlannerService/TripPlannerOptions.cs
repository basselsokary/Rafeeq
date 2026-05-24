namespace Infrastructure.ExternalServices.TripPlannerService;

public class TripPlannerOptions
{
    public const string SectionName = "TripPlannerService";

    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>
    /// Endpoint path on the trip-planner service.
    /// e.g. "/plan" or "/api/v1/plan"
    /// </summary>
    public string PlanEndpoint { get; init; } = "/plan";

    public int TimeoutSeconds { get; init; } = 30;
}