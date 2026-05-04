using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sponsors;

namespace Application.Queries.Sponsors.Offers;

public sealed record GetSponsorOffersByIdQuery(
    Guid SponsorId,
    bool ActiveOnly = true) : IQuery<List<SponsorOfferDto>>;

internal sealed class GetSponsorOffersQueryByIdHandler(
    ISponsorQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetSponsorOffersByIdQuery, List<SponsorOfferDto>>
{
    public async Task<Result<List<SponsorOfferDto>>> HandleAsync(GetSponsorOffersByIdQuery query, CancellationToken cancellationToken)
    {
        var sponsorOfferDtos = await queryService.GetOffersAsync(
            query.SponsorId,
            query.ActiveOnly,
            userContext.Language,
            cancellationToken);
        
        return sponsorOfferDtos;
    }
}
