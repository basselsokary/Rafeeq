using Application.DTOs;

namespace Application.Common.Interfaces.QueryServices;

public interface IDashboardQueryService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
