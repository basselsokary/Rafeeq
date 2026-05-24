using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Artifacts;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class CachedArtifactQueryService(IArtifactQueryService inner, IMemoryCache cache)
    : BaseCache("artifact", cache), IArtifactQueryService
{
    public async Task<ArtifactDetailsDto?> GetByIdAsync(Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:{id}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetByIdAsync(id, language, cancellationToken));
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
}
