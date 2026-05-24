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

        return Result.Success(dashboard);
    }
}