using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sites;

namespace Application.Queries.Sites;

public sealed record GetFeaturedSitesQuery(
    Guid? City = null) : IQuery<List<SiteListDto>>;

internal sealed class GetFeaturedSitesQueryHandler(
    ISiteQueryService queryService,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetFeaturedSitesQuery, List<SiteListDto>>
{
    public async Task<Result<List<SiteListDto>>> HandleAsync(GetFeaturedSitesQuery query, CancellationToken cancellationToken)
    {
        var featuredSites = await queryService.GetFeaturedAsync(
            city: query.City,
            cancellationToken: cancellationToken);

        var localizedData = featuredSites.Select(site => site with
        {
            StatusDisplay = enumLocalizer.Localize(site.Status),
            TypeDisplay = enumLocalizer.Localize(site.Type)
        }).ToList();

        return Result.Success(localizedData);
    }
}

