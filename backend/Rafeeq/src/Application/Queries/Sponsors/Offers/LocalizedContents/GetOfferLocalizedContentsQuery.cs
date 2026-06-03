using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors.Offers.LocalizedContents;

public sealed record GetOfferLocalizedContentsQuery(
    Guid OfferId,
    Guid ContentId) : IQuery<List<AdminOfferLocalizedContentDto>>;

internal sealed class GetOfferLocalizedContentsQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetOfferLocalizedContentsQuery, List<AdminOfferLocalizedContentDto>>
{
    public async Task<Result<List<AdminOfferLocalizedContentDto>>> HandleAsync(GetOfferLocalizedContentsQuery query, CancellationToken cancellationToken)
    {
        var localizedContentDto = await queryService.GetOfferLocalizedContentsAsync(
            query.OfferId,
            cancellationToken);

        if (localizedContentDto == null)
            return SponsorErrors.LocalizedContentNotFound;
    
        return Result.Success(localizedContentDto);
    }
}
