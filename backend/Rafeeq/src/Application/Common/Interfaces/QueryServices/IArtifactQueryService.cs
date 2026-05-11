using Application.DTOs.Artifacts;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IArtifactQueryService
{
    Task<ArtifactDetailsDto?> GetByIdAsync(
        Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    Task<ArtifactDetailsDto?> GetByNameAsync(
        string name, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    Task<List<ArtifactListDto>?> GetByNamesAsync(
        List<string> names, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
}
