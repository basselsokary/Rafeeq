namespace Application.DTOs.Tourists;

public record TouristProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string PreferredLanguage,
    string? Nationality,
    int TotalTrips,
    int TotalReviews,
    DateTime CreatedAt,
    DateTime LastLoginAt);
