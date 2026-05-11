using Domain.Enums;

namespace Application.DTOs.Sponsors;

public record NearbySponsorDto(
    Guid Id,
    string Name,
    SponsorType Type,
    double Latitude,
    double Longitude,
    double DistanceKm,
    string? PrimaryImageUrl,
    double AverageRating,
    bool HasActiveOffers,
    int ActiveOffersCount,
    string TypeDisplay = "");
