using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sites;

namespace Application.Queries.Sites;

public record GetFeaturedSitesQuery(
    Guid? City = null) : IQuery<List<SiteListDto>>;

internal class GetFeaturedSitesQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetFeaturedSitesQuery, List<SiteListDto>>
{
    public async Task<Result<List<SiteListDto>>> HandleAsync(GetFeaturedSitesQuery query, CancellationToken cancellationToken)
    {
        var featuredSites = await queryService.GetFeaturedAsync(
            city: query.City,
            cancellationToken: cancellationToken);

            return Result.Success(featuredSites);
    }
}

