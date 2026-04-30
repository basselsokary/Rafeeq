using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors.Images;

public sealed record GetSponsorImageByIdQuery(Guid SponsorId, Guid ImageId) : IQuery<ImageDto>;

internal sealed class GetSponsorImageByIdQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSponsorImageByIdQuery, ImageDto>
{
    public async Task<Result<ImageDto>> HandleAsync(GetSponsorImageByIdQuery query, CancellationToken cancellationToken)
    {
        var image = await queryService.GetImageByIdAsync(query.SponsorId, query.ImageId, cancellationToken);
        if (image is null)
            return SponsorErrors.ImageNotFound;

        return Result.Success(image);
    }
}
