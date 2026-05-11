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
    
    // Sponsors
    public int TotalSponsors { get; set; }
    public int ActiveSponsors { get; set; }
    public int ActiveOffers { get; set; }
}

/// <summary>
/// System activity log DTO
/// </summary>
public class SystemActivityLogDto
{
    public Guid Id { get; set; }
    public string ActivityType { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public Guid? EntityId { get; set; }
    public Guid? PerformedBy { get; set; }
    public string? PerformedByName { get; set; }
    public string Description { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// System health DTO
/// </summary>
public class SystemHealthDto
{
    public string Status { get; set; } = null!; // Healthy, Warning, Critical
    public double CpuUsagePercent { get; set; }
    public double MemoryUsagePercent { get; set; }
    public double DiskUsagePercent { get; set; }
    public int ActiveConnections { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public int ErrorsLastHour { get; set; }
    public DateTime LastHealthCheck { get; set; }
}

