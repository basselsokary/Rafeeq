using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Application.DTOs.Artifacts;
using Application.DTOs.Common;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class CachedArtifactQueryService(IArtifactQueryService inner, IMemoryCache cache)
    : BaseCache("artifact", cache), IArtifactQueryService
{
    public async Task<bool> AnyAsync(Guid artifactId, CancellationToken cancellationToken)
    {
        return await inner.AnyAsync(artifactId, cancellationToken);
    }

    public async Task<PagedResult<ArtifactListDto>> GetAllAsync(string? searchTerm, ArtifactType? type, PagingParameters paging, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:list:{searchTerm}:{type}:{paging.Page}:{paging.PageSize}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetAllAsync(searchTerm, type, paging, cancellationToken));
    }

    public async Task<ArtifactDetailsDto?> GetByIdAsync(Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:{id}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetByIdAsync(id, language, cancellationToken));
    }

    public async Task<ArtifactAdminDetailDto?> GetByIdForAdminAsync(Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:admin:{id}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetByIdForAdminAsync(id, language, cancellationToken));
    }

    public async Task<ArtifactDetailsDto?> GetByNameAsync(string name, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:{name}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetByNameAsync(name, language, cancellationToken));
    }

    public async Task<List<ArtifactListDto>> GetByNamesAsync(List<string> names, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        return await inner.GetByNamesAsync(names, language, cancellationToken);
    }

    public async Task<List<ArtifactListDto>> GetBySiteIdAsync(Guid siteId, ArtifactType? type, string? searchTerm = null, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:site:{siteId}:{type}:{searchTerm}:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetBySiteIdAsync(siteId, type, searchTerm, language, cancellationToken));
    }

    public async Task<AdminArtifactDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:dashboard";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetDashboardAsync(cancellationToken));
    }

    public async Task<List<ImageDto>> GetImagesAsync(Guid artifactId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:{artifactId}:images";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetImagesAsync(artifactId, cancellationToken));
        
    }

    public async Task<List<ArtifactLocalizedContentDto>> GetLocalizedContentsAsync(Guid artifactId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:{artifactId}:localizedContents";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetLocalizedContentsAsync(artifactId, cancellationToken));
    }
}
