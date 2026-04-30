using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites.Facilities;

public sealed record GetSiteFacilitiesByIdQuery(Guid SiteId) : IQuery<List<AdminSiteFacilityDto>>;

internal sealed class GetSiteFacilitiesByIdQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteFacilitiesByIdQuery, List<AdminSiteFacilityDto>>
{
    public async Task<Result<List<AdminSiteFacilityDto>>> HandleAsync(GetSiteFacilitiesByIdQuery query, CancellationToken cancellationToken)
    {
        var site = await queryService.GetByIdForAdminAsync(query.SiteId, cancellationToken);
        if (site is null)
            return SiteErrors.NotFound(query.SiteId);

        var facilities = await queryService.GetFacilitiesAsync(query.SiteId, cancellationToken);
        return Result.Success(facilities);
    }
}
