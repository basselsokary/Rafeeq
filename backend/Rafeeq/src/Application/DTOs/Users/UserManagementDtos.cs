namespace Application.DTOs.Users;

public record UserListDto(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    string Status,  // Active, Locked, Suspended, Deleted
    bool EmailConfirmed,
    bool PhoneNumberConfirmed,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    DateTime? LockedUntil)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
}

/// <summary>
/// DTO for detailed user information
/// </summary>
public record UserDetailsDto(
    Guid Id,
    string Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed,
    string? FirstName,
    string? LastName,
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

public record UserSearchResultDto(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    string Status)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
}

// ============================================
// ACTIVITY & AUDIT DTOs
// ============================================

/// <summary>
/// DTO for user activity log entries
/// </summary>
public record UserActivityDto(
    Guid Id,
    string ActivityType,  // Login, Logout, PasswordChange, etc.
    string Description,
    string? IpAddress,
    string? UserAgent,
    string? Location,  // City, Country
    DateTime Timestamp);

/// <summary>
/// DTO for login history entries
/// </summary>
public record LoginHistoryDto(
    Guid Id,
    DateTime LoginAt,
    string? IpAddress,
    string? UserAgent,
    string? Device,  // Mobile, Desktop, Tablet
    string? Browser,
    string? Location,  // City, Country
    bool WasSuccessful,
    string? FailureReason);

/// <summary>
/// DTO for user statistics dashboard
/// </summary>
public record UserStatisticsDto(
    int TotalUsers,
    int ActiveUsers,
    int LockedUsers,
    int SuspendedUsers,
    int DeletedUsers,
    UsersGrowthDto UsersGrowth,
    UserActivityStatsDto ActivityStats,
    int UnverifiedEmailsCount,
    int UsersRequiringPasswordChange,
    int UsersWith2FAEnabled);

public record UsersGrowthDto(
    int NewUsersToday,
    int NewUsersThisWeek,
    int NewUsersThisMonth,
    List<DailyUserCountDto>? Last30Days)
{
    public List<DailyUserCountDto> Last30Days { get; init; } = Last30Days ?? new();
}

public record DailyUserCountDto(
    DateTime Date,
    int Count);

public record UserActivityStatsDto(
    int ActiveUsersLast24Hours,
    int ActiveUsersLast7Days,
    int ActiveUsersLast30Days);
