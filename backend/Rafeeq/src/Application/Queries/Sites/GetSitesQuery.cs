using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sites;

namespace Application.Queries.Sites;

public sealed record GetSitesQuery(
    SiteFilters Filters,
    PagingParameters Paging,
    string? SearchTerm = null) : IQuery<PagedResult<SiteListDto>>;

internal sealed class GetSitesQueryHandler(
    ISiteQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetSitesQuery, PagedResult<SiteListDto>>
{
    public async Task<Result<PagedResult<SiteListDto>>> HandleAsync(GetSitesQuery query, CancellationToken cancellationToken)
    {
        PagedResult<SiteListDto> sites;
        if (string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            sites = await queryService.GetAsync(
                query.Filters,
                query.Paging,
                userContext.Language,
                cancellationToken);
        } 
        else
        {
            sites = await queryService.SearchAsync(
                query.SearchTerm,
                query.Filters,
                query.Paging,
                userContext.Language,
                cancellationToken);
        }

        var localizedData = sites.Data.Select(site => site with
        {
            StatusDisplay = enumLocalizer.Localize(site.Status),
            TypeDisplay = enumLocalizer.Localize(site.Type)
        }).ToList();

        return Result.Success(sites with { Data = localizedData });
    }
}