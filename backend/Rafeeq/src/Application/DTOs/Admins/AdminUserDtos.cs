namespace Application.DTOs.Admins;

/// <summary>
/// Admin user list DTO
/// </summary>
public class AdminUserListDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Role { get; set; }
    public string Status { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    
    // Stats
    public int TotalTrips { get; set; }
    public int TotalReviews { get; set; }
    public int TotalVisits { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
}

/// <summary>
/// Admin user detail DTO
/// </summary>
public class AdminUserDetailDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string Role { get; set; }
    public string Status { get; set; }
    public string PreferredLanguage { get; set; }
    public string? Bio { get; set; }
    public string? Nationality { get; set; }
    public DateTime? DateOfBirth { get; set; }
    
    // Verification
    public bool EmailVerified { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public bool PhoneVerified { get; set; }
    public DateTime? PhoneVerifiedAt { get; set; }
    
    // Statistics
    public int TotalTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int TotalReviews { get; set; }
    public int ApprovedReviews { get; set; }
    public int TotalVisits { get; set; }
    public int TotalFavorites { get; set; }
    
    // Preferences
    public List<UserPreferenceDto> Preferences { get; set; } = new();
    
    // Internal
    public string? InternalNotes { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsBanned { get; set; }
    public string? BanReason { get; set; }
    public DateTime? BannedAt { get; set; }
    public Guid? BannedBy { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public int LoginCount { get; set; }
    public int FailedLoginAttempts { get; set; }
}

/// <summary>
/// User preference DTO
/// </summary>
public class UserPreferenceDto
{
    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Admin user statistics
/// </summary>
public class AdminUserStatisticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int SuspendedUsers { get; set; }
    public int DeletedUsers { get; set; }
    
    public Dictionary<string, int> CountByRole { get; set; } = new();
    public Dictionary<string, int> CountByStatus { get; set; } = new();
    
    public int EmailVerifiedUsers { get; set; }
    public int PhoneVerifiedUsers { get; set; }
    
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    
    public int ActiveUsersToday { get; set; }
    public int ActiveUsersThisWeek { get; set; }
    public int ActiveUsersThisMonth { get; set; }
    
    public List<TopUserDto> MostActiveUsers { get; set; } = new();
    public List<TopUserDto> TopReviewers { get; set; } = new();
    public List<TopUserDto> TopTravelers { get; set; } = new();
}

/// <summary>
/// Top user DTO for rankings
/// </summary>
public class TopUserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int Value { get; set; } // Trips, reviews, visits, etc.
    public string Role { get; set; }
}

/// <summary>
/// User activity log DTO
/// </summary>
public class UserActivityLogDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ActivityType { get; set; } // Login, Review, Trip, Visit, etc.
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}
