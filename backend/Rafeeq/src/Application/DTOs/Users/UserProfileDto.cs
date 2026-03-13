namespace Application.DTOs.Users;

/// <summary>
/// User profile DTO
/// </summary>
public record UserProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? ProfileImageUrl,
    string Role,
    string Status,
    string PreferredLanguage,
    string? Bio,
    string? Nationality,
    DateTime? DateOfBirth,
    bool EmailVerified,
    bool PhoneVerified,
    int TotalTrips,
    int TotalReviews,
    DateTime CreatedAt,
    DateTime LastLoginAt);
