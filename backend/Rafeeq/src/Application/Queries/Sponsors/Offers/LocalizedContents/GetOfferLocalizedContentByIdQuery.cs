using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors.Offers.LocalizedContents;

public sealed record GetOfferLocalizedContentByIdQuery(
    Guid OfferId,
    Guid ContentId) : IQuery<AdminOfferLocalizedContentDto>;

internal sealed class GetOfferLocalizedContentByIdQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetOfferLocalizedContentByIdQuery, AdminOfferLocalizedContentDto>
{
    public async Task<Result<AdminOfferLocalizedContentDto>> HandleAsync(GetOfferLocalizedContentByIdQuery query, CancellationToken cancellationToken)
    {
        var localizedContentDto = await queryService.GetOfferLocalizedContentByIdAsync(
            query.OfferId,
            query.ContentId,
            cancellationToken);

        if (localizedContentDto == null)
            return SponsorErrors.LocalizedContentNotFound;
    
        return Result.Success(localizedContentDto);
    }
}
