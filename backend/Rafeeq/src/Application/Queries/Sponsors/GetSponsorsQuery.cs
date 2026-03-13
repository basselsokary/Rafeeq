using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;

namespace Application.Queries.Sponsors;

public record GetSponsorsQuery(
    SponsorFilters Filters,
    string? SearchTerm = null,
    PagingParameters? Paging = null) : IQuery<PagedResult<SponsorListDto>>;

internal class GetSponsorsQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSponsorsQuery, PagedResult<SponsorListDto>>
{
    public async Task<Result<PagedResult<SponsorListDto>>> HandleAsync(GetSponsorsQuery query, CancellationToken cancellationToken)
    {
        PagedResult<SponsorListDto> sponsors;
        if (string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            sponsors = await queryService.GetAsync(
                query.Filters,
                query.Paging,
                cancellationToken);
        } 
        else
        {
            sponsors = await queryService.SearchAsync(
                query.SearchTerm,
                query.Filters,
                query.Paging,
                cancellationToken);
        }
            return Result.Success(sponsors);
    }
}