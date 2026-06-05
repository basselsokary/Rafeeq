using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Artifacts;

namespace Application.Queries.Artifacts.LocalizedContents;

public sealed record GetArtifactLocalizedContentsQuery(
    Guid ArtifactId) : IQuery<List<ArtifactLocalizedContentDto>>;

internal sealed class GetArtifactLocalizedContentsQueryHandler(
    IArtifactQueryService queryService) : IQueryHandler<GetArtifactLocalizedContentsQuery, List<ArtifactLocalizedContentDto>>
{
    public async Task<Result<List<ArtifactLocalizedContentDto>>> HandleAsync(GetArtifactLocalizedContentsQuery query, CancellationToken cancellationToken)
    {
        var localizedContentDtos =
            await queryService.GetLocalizedContentsAsync(query.ArtifactId, cancellationToken);
    
        return localizedContentDtos;
    }
}