using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sites;

namespace Application.Queries.Sites;

public sealed record GetNearbySitesQuery(
    double Latitude,
    double Longitude,
    SiteFilters Filters,
    int RadiusKm = 5) : IQuery<List<SiteListDto>>;

internal sealed class GetNearbySitesQueryHandler(
    ISiteQueryService queryService,
    IEnumLocalizer enumLocalizer,
    IUserContext userContext) : IQueryHandler<GetNearbySitesQuery, List<SiteListDto>>
{
    public async Task<Result<List<SiteListDto>>> HandleAsync(GetNearbySitesQuery query, CancellationToken cancellationToken)
    {
        var siteListDtos = await queryService.GetNearbyAsync(
            query.Latitude,
            query.Longitude,
            query.Filters,
            query.RadiusKm,
            language: userContext.Language,
            cancellationToken: cancellationToken);

        var localizedSiteListDtos = siteListDtos.Select(site => site with
        {
            StatusDisplay = enumLocalizer.Localize(site.Status),
            TypeDisplay = enumLocalizer.Localize(site.Type)
        }).ToList();

        return Result.Success(localizedSiteListDtos);
    }
}