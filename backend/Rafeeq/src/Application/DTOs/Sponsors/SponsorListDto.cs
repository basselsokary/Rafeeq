using Domain.Enums;

namespace Application.DTOs.Sponsors;

public record SponsorListDto(
    Guid Id,
    string Title,
    string Description,
    SponsorType Type,
    double Latitude,
    double Longitude,
    string? PrimaryImageUrl,
    double AverageRating,
    int TotalReviews,
    SponsorStatus Status,
    int ActiveOffersCount,
    double? DistanceKm,
    string TypeDisplay = "");
