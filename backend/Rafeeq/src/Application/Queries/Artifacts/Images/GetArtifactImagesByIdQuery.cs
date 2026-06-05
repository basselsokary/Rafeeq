using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Domain.Entities.ArtifactAggregate;

namespace Application.Queries.Artifacts.Images;

public sealed record GetArtifactImagesByIdQuery(Guid ArtifactId) : IQuery<List<ImageDto>>;

internal sealed class GetArtifactImagesByIdQueryHandler(
    IArtifactQueryService queryService,
    IFileStorageService fileStorageService) : IQueryHandler<GetArtifactImagesByIdQuery, List<ImageDto>>
{
    public async Task<Result<List<ImageDto>>> HandleAsync(GetArtifactImagesByIdQuery query, CancellationToken cancellationToken)
    {
        var artifactExist = await queryService.AnyAsync(query.ArtifactId, cancellationToken);
        if (!artifactExist)
            return ArtifactErrors.NotFound(query.ArtifactId.ToString());

        var images = await queryService.GetImagesAsync(query.ArtifactId, cancellationToken);
        
        return Result.Success(images.Select(i => i with
        {
            StorageKey = i.StorageKey,
            Url = fileStorageService.GetOptimizedUrl(i.StorageKey)
        }).ToList());
    }
}
