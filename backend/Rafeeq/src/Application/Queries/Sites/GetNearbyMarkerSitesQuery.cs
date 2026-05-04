using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sites;

namespace Application.Queries.Sites;

public sealed record GetNearbyMarkerSitesQuery(
    double Latitude,
    double Longitude) : IQuery<List<SiteMapMarkerDto>>;

internal sealed class GetNearbyMarkerSitesQueryHandler(
    ISiteQueryService queryService,
    IEnumLocalizer enumLocalizer,
    IUserContext userContext) : IQueryHandler<GetNearbyMarkerSitesQuery, List<SiteMapMarkerDto>>
{ 
    public async Task<Result<List<SiteMapMarkerDto>>> HandleAsync(GetNearbyMarkerSitesQuery query, CancellationToken cancellationToken)
    {
        var siteListDtos = await queryService.GetNearbyMarkerAsync(
            query.Latitude,
            query.Longitude,
            20,
            10,
            userContext.Language,
            cancellationToken);

        var localizedSiteListDtos = siteListDtos.Select(site => site with
        {
            TypeDisplay = enumLocalizer.Localize(site.Type)
        }).ToList();

        return Result.Success(localizedSiteListDtos);
    }
}