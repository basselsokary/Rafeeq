using Domain.Enums;

namespace Application.DTOs.Tourists;

public record TouristAdminDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    UserStatus Status,
    string? Nationality,
    bool EmailVerified,
    // int TotalTrips,
    // int CompletedTrips,
    int TotalReviews,
    int TotalFavorites,
    bool IsBanned,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? LastLoginAt,
    int FailedLoginAttempts);
