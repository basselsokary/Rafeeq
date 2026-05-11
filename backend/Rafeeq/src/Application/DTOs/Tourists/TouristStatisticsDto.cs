namespace Application.DTOs.Tourists;

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
