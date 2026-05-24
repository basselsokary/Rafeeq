using Application.Common.Interfaces.QueryServices;
using Application.DTOs;

namespace Application.Queries.Dashboard;

public sealed record GetDashboardStatsQuery : IQuery<DashboardStatsDto>;

internal sealed class GetDashboardStatsQueryHandler(
    IDashboardQueryService dashboardService) : IQueryHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<Result<DashboardStatsDto>> HandleAsync(GetDashboardStatsQuery query, CancellationToken cancellationToken)
    {
        var stats = await dashboardService.GetDashboardStatsAsync();

        return stats;
    }
}