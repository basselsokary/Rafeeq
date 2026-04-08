namespace Application.DTOs.Sponsors;

/// <summary>
/// Sponsor list DTO
/// </summary>
public record SponsorListDto(
    Guid Id,
    string Title,
    string ShortDescription,
    string Type,
    string Tier,
    double Latitude,
    double Longitude,
    string? PrimaryImageUrl,
    double AverageRating,
    int TotalReviews,
    bool IsActive,
    int ActiveOffersCount,
    double? DistanceKm);
