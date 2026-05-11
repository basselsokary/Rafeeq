using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Domain.Entities.AttractionAggregate;

namespace Application.Queries.Attractions.Images;

public sealed record GetAttractionImagesByIdQuery(Guid AttractionId) : IQuery<List<ImageDto>>;

internal sealed class GetAttractionImagesByIdQueryHandler(
    IAttractionQueryService queryService,
    IFileStorageService fileStorageService) : IQueryHandler<GetAttractionImagesByIdQuery, List<ImageDto>>
{
    public async Task<Result<List<ImageDto>>> HandleAsync(GetAttractionImagesByIdQuery query, CancellationToken cancellationToken)
    {
        var images = await queryService.GetImagesAsync(query.AttractionId, cancellationToken);
        
        return Result.Success(images.Select(i => i with
        {
            StorageKey = i.StorageKey,
            Url = fileStorageService.GetOptimizedUrl(i.StorageKey)
        }).ToList());
    }
}
