using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;

namespace Application.Queries.Sponsors.Offers;

public sealed record GetAllSponsorsOffersQuery(
    SponsorFilters Filters,
    PagingParameters Paging,
    bool ActiveOnly = true) : IQuery<PagedResult<SponsorOfferListDto>>;

internal sealed class GetAllSponsorsOffersQueryHandler(
    ISponsorQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetAllSponsorsOffersQuery, PagedResult<SponsorOfferListDto>>
{
    public async Task<Result<PagedResult<SponsorOfferListDto>>> HandleAsync(GetAllSponsorsOffersQuery query, CancellationToken cancellationToken)
    {
        var sponsorOfferDtos = await queryService.GetAllOffersAsync(
            query.Filters,
            query.Paging,
            query.ActiveOnly,
            userContext.Language,
            cancellationToken);
        
        return sponsorOfferDtos;
    }
}
