using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sites;

namespace Application.Queries.Sites;

public sealed record SearchSitesQuery(string SearchTerm) : IQuery<List<SiteLookupDto>>;

internal sealed class SearchSitesQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<SearchSitesQuery, List<SiteLookupDto>>
{
    public async Task<Result<List<SiteLookupDto>>> HandleAsync(SearchSitesQuery query, CancellationToken cancellationToken)
    {
        var sites = await queryService.SearchAsync(
            query.SearchTerm,
            10,
            cancellationToken);

        return Result.Success(sites);
    }
}