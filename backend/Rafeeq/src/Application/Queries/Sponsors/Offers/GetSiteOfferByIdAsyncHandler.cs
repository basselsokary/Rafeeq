using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sponsors;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors.Offers;

public record GetSiteOfferByIdAsync(Guid OfferId) : IQuery<SponsorOfferDto>;

internal class GetSiteOfferByIdAsyncHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSiteOfferByIdAsync, SponsorOfferDto>
{
    public async Task<Result<SponsorOfferDto>> HandleAsync(GetSiteOfferByIdAsync query, CancellationToken cancellationToken)
    {
        var sponsorOfferDto = await queryService.GetOfferByIdAsync(query.OfferId, cancellationToken);
        if (sponsorOfferDto == null)
            return SponsorErrors.OfferNotFound(query.OfferId);
        
        return sponsorOfferDto;
    }
}
