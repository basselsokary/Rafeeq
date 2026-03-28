namespace Application.DTOs.Tourists;

/// <summary>
/// Tourist statistics DTO
/// </summary>
public record TouristStatisticsDto(
    Guid TouristId,
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
