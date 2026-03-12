using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sites;

namespace Application.Queries.Sites;

public record GetSitesQuery(
    string? SearchTerm = null,
    SiteFilters? Filters = null,
    PagingParameters? Paging = null) : IQuery<PagedResult<SiteListDto>>;

internal class GetSitesQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSitesQuery, PagedResult<SiteListDto>>
{
    public async Task<Result<PagedResult<SiteListDto>>> HandleAsync(GetSitesQuery query, CancellationToken cancellationToken)
    {
        PagedResult<SiteListDto> sites;
        if (string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            sites = await queryService.GetAllAsync(
                query.Filters,
                query.Paging,
                cancellationToken);
        } 
        else
        {
            sites = await queryService.SearchAsync(
                query.SearchTerm,
                query.Filters,
                query.Paging,
                cancellationToken);
        }
            return Result.Success(sites);
    }
}