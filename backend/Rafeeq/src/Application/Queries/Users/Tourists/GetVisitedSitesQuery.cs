using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Tourists;

namespace Application.Queries.Users.Tourists;

public sealed record GetVisitedSitesQuery(
    PagingParameters Paging) : IQuery<PagedResult<VisitedSiteDto>>;

internal sealed class GetVisitedSitesQueryHandler(
    ITouristQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetVisitedSitesQuery, PagedResult<VisitedSiteDto>>
{
    public async Task<Result<PagedResult<VisitedSiteDto>>> HandleAsync(GetVisitedSitesQuery query, CancellationToken cancellationToken)
    {
        var pagedResult = await queryService.GetVisitedSitesAsync(
            userContext.Id,
            query.Paging,
            userContext.Language,
            cancellationToken);

        var localizedData = pagedResult.Data.Select(visitedSite => visitedSite with
        {
            SiteTypeDisplay = enumLocalizer.Localize(visitedSite.SiteType)
        }).ToList();
    
        return Result.Success(pagedResult with { Data = localizedData });
    }
}