namespace Application.DTOs;

public sealed record DashboardStatsDto(
    int TotalCities,
    int TotalSites,
    int TotalSponsors,
    int TotalUsers);