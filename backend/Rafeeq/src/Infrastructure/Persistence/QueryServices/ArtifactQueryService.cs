using Application.Common.Interfaces.QueryServices;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Admins;
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

    public async Task<ArtifactAdminDetailDto?> GetByIdForAdminAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        var data = await Artifacts.Where(a => a.Id == id)
            .Select(a => new
            {
                a.Id,
                Localized = a.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Name, lc.Description })
                    .FirstOrDefault()!,
                a.SiteId,
                SiteName = a.SiteId == null
                    ? null
                    : a.Site.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                        .OrderBy(lc => lc.Language == language ? 0 : 1)
                        .Select(lc => lc.Name)
                        .FirstOrDefault()!,
                a.MainImageUrl,
                a.Type,
                a.DisplayOrder,
                Images = a.Images
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new ImageDto(
                        i.Id,
                        i.StorageKey,
                        i.ImageUrl,
                        i.Caption,
                        i.IsMain,
                        i.DisplayOrder
                    ))
                    .ToList(),
                a.CreatedAt,
                a.CreatedBy,
                a.LastModifiedAt,
                a.LastModifiedBy
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (data == null)
            return null;

        return new ArtifactAdminDetailDto(
            data.Id,
            data.SiteId,
            data.Localized.Name,
            data.Localized.Description,
            data.SiteName,
            data.MainImageUrl,
            data.Type,
            data.DisplayOrder,
            data.Images,
            data.CreatedAt,
            data.CreatedBy,
            data.LastModifiedAt,
            data.LastModifiedBy);
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
                SiteName = a.SiteId == null
                    ? null
                    : a.Site.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                        .OrderBy(lc => lc.Language == language ? 0 : 1)
                        .Select(lc => lc.Name)
                        .FirstOrDefault()!,
                a.MainImageUrl,
                a.Type,
                Images = a.Images
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new ImageDto(
                        i.Id,
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
            data.SiteName ?? string.Empty,
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

    public async Task<List<ArtifactListDto>> GetBySiteIdAsync(
        Guid siteId,
        ArtifactType? type,
        string? searchTerm = null,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Artifacts.Where(a => a.SiteId == siteId);

        if (type != null)
        {
            query = query.Where(a => a.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.LocalizedContents.Any(lc =>
                    lc.Language == language &&
                    EF.Functions.Like(lc.Name, $"%{searchTerm}%")));
        }

        return await query
            .OrderBy(a => a.DisplayOrder)
            .Select(a => new ArtifactListDto(
                a.Id,
                a.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                a.MainImageUrl,
                a.Type))
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<ArtifactListDto>> GetAllAsync(
        string? searchTerm,
        ArtifactType? type,
        PagingParameters paging,
        CancellationToken cancellationToken)
    {
        var query = Artifacts;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.LocalizedContents.Any(lc =>
                    EF.Functions.Like(lc.Name, $"%{searchTerm}%")));
        }

        if (type != null)
        {
            query = query.Where(a => a.Type == type);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(a => a.DisplayOrder)
            .Skip(paging.Skip)
            .Take(paging.Take)
            .Select(a => new ArtifactListDto(
                a.Id,
                a.LocalizedContents.Where(lc => lc.Language == LanguageCode.English)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                a.MainImageUrl,
                a.Type))
            .ToListAsync(cancellationToken);

        return new PagedResult<ArtifactListDto>(
            items,
            totalCount,
            paging.Page,
            paging.PageSize);
    }

    public async Task<List<ArtifactLocalizedContentDto>> GetLocalizedContentsAsync(
        Guid artifactId,
        CancellationToken cancellationToken = default)
    {
        return await Artifacts
            .Where(a => a.Id == artifactId)
            .SelectMany(a => a.LocalizedContents)
            .Select(lc => new ArtifactLocalizedContentDto(
                lc.Id,
                lc.Language,
                lc.Name,
                lc.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ImageDto>> GetImagesAsync(
        Guid artifactId,
        CancellationToken cancellationToken = default)
    {
        return await Artifacts
            .Where(a => a.Id == artifactId)
            .SelectMany(a => a.Images)
            .Select(i => new ImageDto(
                i.Id,
                i.StorageKey,
                i.ImageUrl,
                i.Caption,
                i.IsMain,
                i.DisplayOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminArtifactDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var dashboardData = await Artifacts
            .GroupBy(_ => 1)
            .Select(g => new AdminArtifactDashboardDto(
                TotalArtifacts: g.Count(),
                AssignedToSites: g.Count(a => a.SiteId != null)
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return dashboardData ?? new AdminArtifactDashboardDto(0, 0);
    }

    public async Task<bool> AnyAsync(Guid artifactId, CancellationToken cancellationToken)
    {
        return await context.Artifacts.AnyAsync(a => a.Id == artifactId, cancellationToken);
    }
}
