namespace Application.DTOs.Tourists;

public record TouristListDto(
    Guid Id,
    string UserName,
    string FullName,
    string Email,
    int TotalTrips,
    int TotalReviews);
