namespace Application.DTOs.Tourists;

/// <summary>
/// Lightweight user DTO for lists
/// </summary>
public record TouristListDto(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    int TotalTrips,
    int TotalReviews);
