namespace Application.DTOs.Admins;



/// <summary>
/// Admin tourist list DTO
/// </summary>
public record AdminTouristListDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string Role,
    string Status,
    bool EmailVerified,
    // Stats
    int TotalTrips,
    int TotalReviews,
    int TotalVisits,
    // Audit
    DateTime CreatedAt,
    DateTime LastLoginAt
);

/// <summary>
/// Admin tourist detail DTO
/// </summary>
public record AdminTouristDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? ProfileImageUrl,
    string Role,
    string Status,
    string? Nationality,
    // Verification
    bool EmailVerified,
    DateTime? EmailVerifiedAt,
    // Statistics
    int TotalTrips,
    int CompletedTrips,
    int TotalRating,
    int TotalVisits,
    int TotalFavorites,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime LastLoginAt
);

/// <summary>
/// Admin tourist statistics
/// </summary>
public record AdminTouristStatisticsDto(
    int TotalTourists,
    int ActiveTourists,
    int InactiveTourists,
    int SuspendedUsers,
    int DeletedUsers,
    Dictionary<string,int> CountByRole,
    Dictionary<string,int> CountByStatus,
    int NewUsersToday,
    int NewUsersThisWeek,
    int NewUsersThisMonth,
    int ActiveUsersToday,
    int ActiveUsersThisWeek,
    int ActiveUsersThisMonth
);

/// <summary>
/// User activity log DTO
/// </summary>
public record UserActivityLogDto(
    Guid Id,
    Guid UserId,
    string ActivityType,
    string Description,
    DateTime Timestamp,
    string? IpAddress,
    string? UserAgent,
    Dictionary<string,string>? Metadata
);
