using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;

namespace Application.Queries.Sponsors.Offers;

public record GetAllSiteOffersAsync(
    SponsorFilters Filters,
    bool ActiveOnly = true,
    PagingParameters? Paging = null) : IQuery<PagedResult<SponsorOfferListDto>>;

internal class GetAllSiteOffersAsyncHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetAllSiteOffersAsync, PagedResult<SponsorOfferListDto>>
{
    public async Task<Result<PagedResult<SponsorOfferListDto>>> HandleAsync(GetAllSiteOffersAsync query, CancellationToken cancellationToken)
    {
        var sponsorOfferDtos = await queryService.GetAllOffersAsync(
            query.Filters,
            query.ActiveOnly,
            query.Paging,
            cancellationToken);
        
        return sponsorOfferDtos;
    }
}
