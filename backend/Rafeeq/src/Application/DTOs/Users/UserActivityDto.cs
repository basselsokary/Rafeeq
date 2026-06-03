namespace Application.DTOs.Users;

public record UserActivityDto(
    Guid Id,
    string ActivityType,  // Login, Logout, PasswordChange, etc.
    string Description,
    string? IpAddress,
    string? UserAgent,
    string? Location,  // City, Country
    DateTime Timestamp);
