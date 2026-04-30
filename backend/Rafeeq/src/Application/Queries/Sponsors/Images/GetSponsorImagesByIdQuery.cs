using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors.Images;

public sealed record GetSponsorImagesByIdQuery(Guid SponsorId) : IQuery<List<ImageDto>>;

internal sealed class GetSponsorImagesByIdQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSponsorImagesByIdQuery, List<ImageDto>>
{
    public async Task<Result<List<ImageDto>>> HandleAsync(GetSponsorImagesByIdQuery query, CancellationToken cancellationToken)
    {
        var sponsor = await queryService.GetByIdForAdminAsync(query.SponsorId, cancellationToken);
        if (sponsor is null)
            return SponsorErrors.NotFound(query.SponsorId);

        var images = await queryService.GetImagesAsync(query.SponsorId, cancellationToken);
        return Result.Success(images);
    }
}
