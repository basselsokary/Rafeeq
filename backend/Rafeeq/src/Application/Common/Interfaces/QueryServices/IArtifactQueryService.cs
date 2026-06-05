using Application.DTOs.Admins;
using Application.DTOs.Artifacts;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IArtifactQueryService
{
    Task<ArtifactDetailsDto?> GetByIdAsync(
        Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    Task<ArtifactAdminDetailDto?> GetByIdForAdminAsync(
        Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    Task<ArtifactDetailsDto?> GetByNameAsync(
        string name, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    Task<List<ArtifactListDto>> GetByNamesAsync(
        List<string> names, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    Task<List<ArtifactListDto>> GetBySiteIdAsync(
        Guid siteId,
        ArtifactType? type,
        string? searchTerm = null,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    Task<PagedResult<ArtifactListDto>> GetAllAsync(
        string? searchTerm,
        ArtifactType? type,
        PagingParameters paging,
        CancellationToken cancellationToken);
    Task<List<ArtifactLocalizedContentDto>> GetLocalizedContentsAsync(
        Guid artifactId,
        CancellationToken cancellationToken = default);
    Task<List<ImageDto>> GetImagesAsync(
        Guid artifactId,
        CancellationToken cancellationToken = default);
    Task<AdminArtifactDashboardDto> GetDashboardAsync(CancellationToken cancellationToken);
    Task<bool> AnyAsync(Guid artifactId, CancellationToken cancellationToken);
}
