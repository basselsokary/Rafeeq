namespace Application.DTOs.Tourists;

public record TouristAdminDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string Status,
    string PreferredLanguage,
    string? Nationality,
    bool EmailVerified,
    DateTime? EmailVerifiedAt,
    // int TotalTrips,
    // int CompletedTrips,
    int TotalReviews,
    int TotalFavorites,
    bool IsBanned,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime LastLoginAt,
    int FailedLoginAttempts);
