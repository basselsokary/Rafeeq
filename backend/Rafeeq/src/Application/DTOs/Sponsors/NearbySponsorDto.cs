namespace Application.DTOs.Sponsors;

/// <summary>
/// Nearby sponsor DTO
/// </summary>
public record NearbySponsorDto(
    Guid Id,
    string Name,
    string Type,
    double Latitude,
    double Longitude,
    double DistanceKm,
    string? PrimaryImageUrl,
    double AverageRating,
    bool HasActiveOffers,
    int ActiveOffersCount);
