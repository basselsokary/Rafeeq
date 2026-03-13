using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sites;

namespace Application.Queries.Sites;

public record GetNearbySitesQuery(
    double Latitude,
    double Longitude,
    int RadiusKm = 5,
    SiteFilters? Filters = null,
    PagingParameters? Paging = null) : IQuery<List<SiteListDto>>;

internal class GetNearbySitesQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetNearbySitesQuery, List<SiteListDto>>
{
    public async Task<Result<List<SiteListDto>>> HandleAsync(GetNearbySitesQuery query, CancellationToken cancellationToken)
    {
        var siteListDtos = await queryService.GetNearbyAsync(
            query.Latitude,
            query.Longitude,
            query.RadiusKm,
            query.Filters,
            cancellationToken);

            return Result.Success(siteListDtos);
    }
}