namespace Application.DTOs.Users;

/// <summary>
/// User statistics DTO
/// </summary>
public record UserStatisticsDto(
    Guid UserId,
    int TotalTrips,
    int CompletedTrips,
    int PlannedTrips,
    int TotalReviews,
    int TotalVisitedAttractions,
    int TotalFavoriteAttractions,
    double AverageRatingGiven,
    int TotalDistanceTraveledKm,
    List<string> MostVisitedAttractionTypes,
    List<string> MostVisitedCities);
