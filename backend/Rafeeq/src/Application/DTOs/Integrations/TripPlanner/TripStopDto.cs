namespace Application.DTOs.Integrations.TripPlanner;

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
