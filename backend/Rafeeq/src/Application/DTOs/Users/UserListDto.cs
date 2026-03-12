namespace Application.DTOs.Users;

/// <summary>
/// Lightweight user DTO for lists
/// </summary>
public record UserListDto(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    int TotalTrips,
    int TotalReviews);
