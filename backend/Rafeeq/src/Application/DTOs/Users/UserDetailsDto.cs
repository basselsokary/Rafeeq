namespace Application.DTOs.Users;

public record UserDetailsDto(
    Guid Id,
    string Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed,
    string FirstName,
    string LastName,
    string? Nationality,
    List<string>? Roles,
    string Status,
    bool TwoFactorEnabled,
    bool LockoutEnabled,
    int AccessFailedCount,
    DateTime? LockoutEnd,
    bool MustChangePassword,
    DateTime? LastPasswordChangedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? LastLoginAt,
    DateTime? DeletedAt,
    TouristDetailsDto? TouristDetails) // Include tourist-specific details if the user is a tourist
{
    public List<string> Roles { get; init; } = Roles ?? new();
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public record TouristDetailsDto(
    int TotalTrips,
    int TotalRatings);