using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sites;

namespace Application.Queries.Sites;

public sealed record GetSitesWithinBoundsQuery(
    BoundingBox Bounds,
    SiteFilters Filters) : IQuery<List<SiteMapMarkerDto>>;

internal sealed class GetSitesWithinBoundsQueryHandler(
    ISiteQueryService queryService,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetSitesWithinBoundsQuery, List<SiteMapMarkerDto>>
{
    public async Task<Result<List<SiteMapMarkerDto>>> HandleAsync(GetSitesWithinBoundsQuery query, CancellationToken cancellationToken)
    {
        var siteMapMarkerDtos = await queryService.GetWithinBoundsAsync(
            query.Bounds,
            query.Filters,
            cancellationToken: cancellationToken);
        
        var localizedData = siteMapMarkerDtos.Select(site => site with
        {
            TypeDisplay = enumLocalizer.Localize(site.Type)
        }).ToList();

        return Result.Success(localizedData);
    }
}

