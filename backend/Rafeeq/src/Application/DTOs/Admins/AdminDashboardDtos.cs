namespace Application.DTOs.Admins;

// ============================================================================
// DASHBOARD DTOs
// ============================================================================

/// <summary>
/// Admin dashboard overview DTO
/// </summary>
public class AdminDashboardOverviewDto
{
    // Users
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    
    // Attractions
    public int TotalAttractions { get; set; }
    public int ActiveAttractions { get; set; }
    public int PendingApprovalAttractions { get; set; }
    
    // Trips
    public int TotalTrips { get; set; }
    public int ActiveTrips { get; set; }
    public int CompletedTrips { get; set; }
    
    // Reviews
    public int TotalReviews { get; set; }
    public int PendingReviews { get; set; }
    public int FlaggedReviews { get; set; }
    
    // Sponsors
    public int TotalSponsors { get; set; }
    public int ActiveSponsors { get; set; }
    public int ActiveOffers { get; set; }
    
    // Emergencies
    public int TotalEmergencyReports { get; set; }
    public int ActiveEmergencyReports { get; set; }
    public int UnresolvedEmergencyReports { get; set; }
    
    // Activity metrics
    public int TotalVisitsToday { get; set; }
    public int TotalVisitsThisWeek { get; set; }
    public int TotalVisitsThisMonth { get; set; }
}

/// <summary>
/// System activity log DTO
/// </summary>
public class SystemActivityLogDto
{
    public Guid Id { get; set; }
    public string ActivityType { get; set; }
    public string EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public Guid? PerformedBy { get; set; }
    public string? PerformedByName { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Analytics data DTO
/// </summary>
public class AnalyticsDataDto
{
    public DateTime Date { get; set; }
    public int Value { get; set; }
}

/// <summary>
/// Chart data DTO
/// </summary>
public class ChartDataDto
{
    public string Label { get; set; }
    public List<AnalyticsDataDto> Data { get; set; } = new();
}

/// <summary>
/// Popular items DTO
/// </summary>
public class PopularItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Count { get; set; }
    public double? Rating { get; set; }
}

/// <summary>
/// Pending tasks DTO
/// </summary>
public class PendingTaskDto
{
    public Guid Id { get; set; }
    public string TaskType { get; set; } // Review, Approval, Report, etc.
    public string EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string EntityName { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public TimeSpan Age { get; set; }
}

/// <summary>
/// System health DTO
/// </summary>
public class SystemHealthDto
{
    public string Status { get; set; } // Healthy, Warning, Critical
    public double CpuUsagePercent { get; set; }
    public double MemoryUsagePercent { get; set; }
    public double DiskUsagePercent { get; set; }
    public int ActiveConnections { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public int ErrorsLastHour { get; set; }
    public DateTime LastHealthCheck { get; set; }
}

/// <summary>
/// Revenue statistics DTO (for sponsors)
/// </summary>
public class RevenueStatisticsDto
{
    public decimal TotalRevenueThisMonth { get; set; }
    public decimal TotalRevenueThisYear { get; set; }
    public string Currency { get; set; }
    public decimal GrowthPercentage { get; set; }
    public List<MonthlyRevenueDto> MonthlyBreakdown { get; set; } = new();
    public Dictionary<string, decimal> RevenueByTier { get; set; } = new();
}

/// <summary>
/// Monthly revenue DTO
/// </summary>
public class MonthlyRevenueDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; }
    public decimal Revenue { get; set; }
    public string Currency { get; set; }
}
