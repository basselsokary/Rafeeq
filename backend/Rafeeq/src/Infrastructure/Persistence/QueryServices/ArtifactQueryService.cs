using Application.Common.Interfaces.QueryServices;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Common;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using Application.DTOs.Artifacts;
using Domain.Entities.ArtifactAggregate;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class ArtifactQueryService(
    ApplicationDbContext context) : IArtifactQueryService
{
    private IQueryable<Artifact> Artifacts => context.Artifacts.AsNoTracking();
    
    public async Task<ArtifactDetailsDto?> GetByIdAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        var query = Artifacts.Where(a => a.Id == id);
        return await GetSingleAsync(query, language, cancellationToken);
    }

    public async Task<ArtifactDetailsDto?> GetByNameAsync(string name, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;
        
        name = name.Trim();
        var query = Artifacts.Where(
                a => a.LocalizedContents.Any(
                    lc => (lc.Language == language || lc.Language == LanguageCode.English) && lc.Name == name));

        return await GetSingleAsync(query, language, cancellationToken);
    }

    private async Task<ArtifactDetailsDto?> GetSingleAsync(
        IQueryable<Artifact> query, LanguageCode language, CancellationToken cancellationToken)
    {
        var data = await query
            .Select(a => new
            {
                a.Id,
                Localized = a.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Name, lc.Description })
                    .FirstOrDefault()!,
                a.SiteId,
                SiteName = a.Site.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                a.MainImageUrl,
                a.Type,
                Images = a.Images
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new ImageDto(
                        default,
                        i.StorageKey,
                        i.ImageUrl,
                        i.Caption,
                        i.IsMain,
                        i.DisplayOrder
                    ))
                    .ToList(),
            }).FirstOrDefaultAsync(cancellationToken);

        if (data == null)
            return null;

        return new ArtifactDetailsDto(
            data.Id,
            data.SiteId ?? Guid.Empty,
            data.Localized.Name,
            data.Localized.Description,
            data.SiteName,
            data.MainImageUrl,
            data.Type,
            data.Images

        );
    }

    public async Task<List<ArtifactListDto>> GetByNamesAsync(List<string> names, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        if (names == null || names.Count == 0)
            return [];
        
        var trimmedNames = names.Select(n => n.Trim()).ToList();
        var query = Artifacts.Where(
                a => a.LocalizedContents.Any(
                    lc => (lc.Language == language || lc.Language == LanguageCode.English) && trimmedNames.Contains(lc.Name)));

        return await query.Select(a => new ArtifactListDto(
            a.Id,
            a.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                .OrderBy(lc => lc.Language == language ? 0 : 1)
                .Select(lc => lc.Name)
                .FirstOrDefault()!,
            a.MainImageUrl,
            a.Type
        )).ToListAsync(cancellationToken);
    }
}
