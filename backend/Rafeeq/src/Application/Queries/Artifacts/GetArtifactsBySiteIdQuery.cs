using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Authentication;
using Application.DTOs.Artifacts;
using Domain.Enums;

namespace Application.Queries.Artifacts;

public sealed record GetArtifactsBySiteIdQuery(
    Guid SiteId,
    ArtifactType? Type,
    string? SearchTerm = null) : IQuery<List<ArtifactListDto>>;

internal sealed class GetArtifactsBySiteIdQueryHandler(
    IArtifactQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetArtifactsBySiteIdQuery, List<ArtifactListDto>>
{
    public async Task<Result<List<ArtifactListDto>>> HandleAsync(GetArtifactsBySiteIdQuery query, CancellationToken cancellationToken)
    {
        List<ArtifactListDto> artifacts = await queryService.GetBySiteIdAsync(
            query.SiteId,
            query.Type,
            query.SearchTerm,
            userContext.Language,
            cancellationToken);

        var localizedArtifacts = artifacts.Select(artifact => artifact with
        {
            TypeDisplay = enumLocalizer.Localize(artifact.Type)
        }).ToList();

        return Result.Success(localizedArtifacts);
    }
}