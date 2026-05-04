using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;

namespace Application.Queries.Sponsors;

public sealed record GetSponsorsQuery(
    SponsorFilters Filters,
    PagingParameters Paging,
    string? SearchTerm = null) : IQuery<PagedResult<SponsorListDto>>;

internal sealed class GetSponsorsQueryHandler(
    ISponsorQueryService queryService,
    IEnumLocalizer enumLocalizer,
    IUserContext userContext) : IQueryHandler<GetSponsorsQuery, PagedResult<SponsorListDto>>
{
    public async Task<Result<PagedResult<SponsorListDto>>> HandleAsync(GetSponsorsQuery query, CancellationToken cancellationToken)
    {
        PagedResult<SponsorListDto> sponsors;
        if (string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            sponsors = await queryService.GetAsync(
                query.Filters,
                query.Paging,
                userContext.Language,
                cancellationToken);
        } 
        else
        {
            sponsors = await queryService.SearchAsync(
                query.SearchTerm,
                query.Filters,
                query.Paging,
                userContext.Language,
                cancellationToken);
        }
        
        var localizedSponsorListDtos = sponsors.Data.Select(sponsor => sponsor with
        {
            TypeDisplay = enumLocalizer.Localize(sponsor.Type)
        }).ToList();

        return sponsors with { Data = localizedSponsorListDtos };
    }
}