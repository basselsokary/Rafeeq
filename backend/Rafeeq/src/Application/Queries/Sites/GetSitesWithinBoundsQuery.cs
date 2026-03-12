using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sites;

namespace Application.Queries.Sites;

public record GetSitesWithinBoundsQuery(
    BoundingBox Bounds,
    SiteFilters? Filters = null) : IQuery<List<SiteMapMarkerDto>>;

internal class GetSitesWithinBoundsQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSitesWithinBoundsQuery, List<SiteMapMarkerDto>>
{
    public async Task<Result<List<SiteMapMarkerDto>>> HandleAsync(GetSitesWithinBoundsQuery query, CancellationToken cancellationToken)
    {
        var siteMapMarkerDtos = await queryService.GetWithinBoundsAsync(
            query.Bounds,
            query.Filters,
            cancellationToken: cancellationToken);

            return Result.Success(siteMapMarkerDtos);
    }
}

