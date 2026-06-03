using Application.DTOs.Admins;

namespace Application.Common.Interfaces.QueryServices;

public interface IDashboardQueryService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
