using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Artifacts;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Queries.Artifacts;

public sealed record GetAllArtifactsQuery(
    string? SearchTerm,
    ArtifactType? Type,
    PagingParameters Paging) : IQuery<PagedResult<ArtifactListDto>>;

internal sealed class GetAllArtifactsQueryHandler(
    IArtifactQueryService queryService,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetAllArtifactsQuery, PagedResult<ArtifactListDto>>
{
    public async Task<Result<PagedResult<ArtifactListDto>>> HandleAsync(GetAllArtifactsQuery query, CancellationToken cancellationToken)
    {
        var artifacts = await queryService.GetAllAsync(query.SearchTerm, query.Type, query.Paging, cancellationToken);

        var localizedData = artifacts.Data.Select(artifact => artifact with
        {
            TypeDisplay = enumLocalizer.Localize(artifact.Type)
        }).ToList();

        return Result.Success(artifacts with { Data = localizedData });
    }
}