using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;

namespace Application.Queries.Sites;

public sealed record GetSitesDashboardQuery() : IQuery<AdminSiteDashboardDto>;

internal sealed class GetSitesDashboardQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSitesDashboardQuery, AdminSiteDashboardDto>
{
    public async Task<Result<AdminSiteDashboardDto>> HandleAsync(GetSitesDashboardQuery query, CancellationToken cancellationToken)
    {
        var dashboard = await queryService.GetDashboardAsync(cancellationToken);
        if (dashboard == null)
            return Result.Failure<AdminSiteDashboardDto>("Failed to retrieve site dashboard data.");

        return Result.Success(dashboard);
    }
}