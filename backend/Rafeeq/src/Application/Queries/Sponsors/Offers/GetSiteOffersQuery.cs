using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sponsors;

namespace Application.Queries.Sponsors.Offers;

public record GetSiteOffersQuery(
    Guid SponsorId,
    bool ActiveOnly = true
) : IQuery<List<SponsorOfferDto>>;

internal class GetSiteOffersQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSiteOffersQuery, List<SponsorOfferDto>>
{
    public async Task<Result<List<SponsorOfferDto>>> HandleAsync(GetSiteOffersQuery query, CancellationToken cancellationToken)
    {
        var sponsorOfferDtos = await queryService.GetOffersAsync(query.SponsorId, query.ActiveOnly, cancellationToken);
        
        return sponsorOfferDtos;
    }
}
