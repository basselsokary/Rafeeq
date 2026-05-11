using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;

namespace Application.Queries.Attractions;

public sealed record GetAttractionsDashboardQuery() : IQuery<AdminAttractionDashboardDto>;

internal sealed class GetAttractionsDashboardQueryHandler(
    IAttractionQueryService queryService) : IQueryHandler<GetAttractionsDashboardQuery, AdminAttractionDashboardDto>
{
    public async Task<Result<AdminAttractionDashboardDto>> HandleAsync(GetAttractionsDashboardQuery query, CancellationToken cancellationToken)
    {
        var dashboard = await queryService.GetDashboardAsync(cancellationToken);
        if (dashboard == null)
            return Result.Failure<AdminAttractionDashboardDto>("Failed to retrieve site dashboard data.");

        return Result.Success(dashboard);
    }
}