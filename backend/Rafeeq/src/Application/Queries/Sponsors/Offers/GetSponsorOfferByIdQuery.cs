using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sponsors;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors.Offers;

public sealed record GetSponsorOfferByIdQuery(Guid OfferId) : IQuery<SponsorOfferDto>;

internal sealed class GetSponsorOfferByIdQueryHandler(
    ISponsorQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetSponsorOfferByIdQuery, SponsorOfferDto>
{
    public async Task<Result<SponsorOfferDto>> HandleAsync(GetSponsorOfferByIdQuery query, CancellationToken cancellationToken)
    {
        var sponsorOfferDto = await queryService.GetOfferByIdAsync(
            query.OfferId,
            userContext.Language,
            cancellationToken);

        if (sponsorOfferDto == null)
            return SponsorErrors.OfferNotFound(query.OfferId);
        
        return sponsorOfferDto;
    }
}
