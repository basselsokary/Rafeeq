using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;

namespace Application.Queries.Artifacts;

public sealed record GetArtifactsDashboardQuery() : IQuery<AdminArtifactDashboardDto>;

internal sealed class GetArtifactsDashboardQueryHandler(
    IArtifactQueryService queryService) : IQueryHandler<GetArtifactsDashboardQuery, AdminArtifactDashboardDto>
{
    public async Task<Result<AdminArtifactDashboardDto>> HandleAsync(GetArtifactsDashboardQuery query, CancellationToken cancellationToken)
    {
        var dashboard = await queryService.GetDashboardAsync(cancellationToken);
        if (dashboard == null)
            return Result.Failure<AdminArtifactDashboardDto>("Failed to retrieve site dashboard data.");

        return Result.Success(dashboard);
    }
}