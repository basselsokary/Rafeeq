using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Domain.Entities.AttractionAggregate;

namespace Application.Queries.Attractions.Images;

public sealed record GetAttractionImagesByIdQuery(Guid AttractionId) : IQuery<List<ImageDto>>;

internal sealed class GetAttractionImagesByIdQueryHandler(
    IAttractionQueryService queryService) : IQueryHandler<GetAttractionImagesByIdQuery, List<ImageDto>>
{
    public async Task<Result<List<ImageDto>>> HandleAsync(GetAttractionImagesByIdQuery query, CancellationToken cancellationToken)
    {
        var attraction = await queryService.GetByIdForAdminAsync(query.AttractionId, cancellationToken: cancellationToken);
        if (attraction is null)
            return AttractionErrors.NotFound(query.AttractionId);

        var images = await queryService.GetImagesAsync(query.AttractionId, cancellationToken);
        return Result.Success(images);
    }
}
