using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Authentication;
using Domain.Entities.ArtifactAggregate;
using Application.DTOs.Admins;

namespace Application.Queries.Artifacts;

public sealed record GetArtifactByIdForAdminQuery(Guid Id) : IQuery<ArtifactAdminDetailDto>;

internal sealed class GetArtifactByIdForAdminQueryHandler(
    IArtifactQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetArtifactByIdForAdminQuery, ArtifactAdminDetailDto>
{
    public async Task<Result<ArtifactAdminDetailDto>> HandleAsync(GetArtifactByIdForAdminQuery query, CancellationToken cancellationToken)
    {
        var artifactDto = await queryService.GetByIdForAdminAsync(
            query.Id,
            userContext.Language,
            cancellationToken);
        if (artifactDto == null)
            return ArtifactErrors.NotFound(query.Id.ToString());

        return Result.Success(artifactDto);

    }
}