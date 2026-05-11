using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;

namespace Application.Queries.Cities;

public sealed record GetCityDashboardQuery() : IQuery<AdminCityDashboardDto>;

internal sealed class GetCityDashboardQueryHandler(
    ICityQueryService queryService) : IQueryHandler<GetCityDashboardQuery, AdminCityDashboardDto>
{
    public async Task<Result<AdminCityDashboardDto>> HandleAsync(GetCityDashboardQuery query, CancellationToken cancellationToken)
    {
        var dashboard = await queryService.GetDashboardAsync(cancellationToken);
        if (dashboard == null)
            return Result.Failure<AdminCityDashboardDto>("Failed to retrieve city dashboard data.");

        return Result.Success(dashboard);
    }
}