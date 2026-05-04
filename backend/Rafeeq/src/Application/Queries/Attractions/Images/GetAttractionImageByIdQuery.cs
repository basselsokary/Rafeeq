using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Domain.Entities.AttractionAggregate;

namespace Application.Queries.Attractions.Images;

public sealed record GetAttractionImageByIdQuery(Guid AttractionId, Guid ImageId) : IQuery<ImageDto>;

internal sealed class GetAttractionImageByIdQueryHandler(
    IAttractionQueryService queryService) : IQueryHandler<GetAttractionImageByIdQuery, ImageDto>
{
    public async Task<Result<ImageDto>> HandleAsync(GetAttractionImageByIdQuery query, CancellationToken cancellationToken)
    {
        var image = await queryService.GetImageByIdAsync(query.AttractionId, query.ImageId, cancellationToken);
        if (image is null)
            return AttractionErrors.ImageNotFound;

        return Result.Success(image);
    }
}
