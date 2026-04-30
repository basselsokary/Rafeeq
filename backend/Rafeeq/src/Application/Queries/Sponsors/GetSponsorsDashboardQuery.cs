using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;

namespace Application.Queries.Sponsors;

public sealed record GetSponsorsDashboardQuery() : IQuery<AdminSponsorDashboardDto>;

internal sealed class GetSponsorsDashboardQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSponsorsDashboardQuery, AdminSponsorDashboardDto>
{
    public async Task<Result<AdminSponsorDashboardDto>> HandleAsync(GetSponsorsDashboardQuery query, CancellationToken cancellationToken)
    {
        var dashboard = await queryService.GetDashboardAsync(cancellationToken);
        if (dashboard == null)
            return Result.Failure<AdminSponsorDashboardDto>("Failed to retrieve Sponsor dashboard data.");

        return Result.Success(dashboard);
    }
}