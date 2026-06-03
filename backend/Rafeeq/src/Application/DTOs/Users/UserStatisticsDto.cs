namespace Application.DTOs.Users;

public record UserStatisticsDto(
    int TotalUsers,
    int ActiveUsers,
    int LockedUsers,
    int SuspendedUsers,
    int DeletedUsers,
    UsersGrowthDto UsersGrowth,
    UserActivityStatsDto ActivityStats,
    UsersByRoleDto UsersByRole,
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

public record UsersByRoleDto(
    int Admins,
    int Moderators,
    int Tourists);

public record DailyUserCountDto(
    DateTime Date,
    int Count);

public record UserActivityStatsDto(
    int ActiveUsersLast24Hours,
    int ActiveUsersLast7Days,
    int ActiveUsersLast30Days);
