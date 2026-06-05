using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Authentication;
using Application.DTOs.Artifacts;
using Domain.Entities.ArtifactAggregate;

namespace Application.Queries.Artifacts;

public sealed record GetArtifactByIdQuery(Guid Id) : IQuery<ArtifactDetailsDto>;

internal sealed class GetArtifactByIdQueryHandler(
    IArtifactQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetArtifactByIdQuery, ArtifactDetailsDto>
{
    public async Task<Result<ArtifactDetailsDto>> HandleAsync(GetArtifactByIdQuery query, CancellationToken cancellationToken)
    {
        var artifactDto = await queryService.GetByIdAsync(
            query.Id,
            userContext.Language,
            cancellationToken);
        if (artifactDto == null)
            return ArtifactErrors.NotFound(query.Id.ToString());

        return Result.Success(
            artifactDto with
            {
                TypeDisplay = enumLocalizer.Localize(artifactDto.Type)
            });

    }
}