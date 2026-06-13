using Application.Common.Interfaces.QueryServices;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.AttractionAggregate;
using Application.DTOs.Admins;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class AttractionQueryService(
    ApplicationDbContext context) : IAttractionQueryService
{
    private IQueryable<Attraction> Attractions => context.Attractions.AsNoTracking();

    public async Task<AttractionDetailDto?> GetByIdAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var data = await Attractions.Where(a => a.Id == id)
            .Select(a => new {
                a.Id,
                Localized = a.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Name, lc.Description, lc.LocationDescription })
                    .FirstOrDefault()!,
                a.Type,
                a.Location,
                HistoricalPeriods = a.HistoricalPeriods.ToList(),
                images = a.Images.Select(i => new ImageDto(i.Id, i.StorageKey, i.ImageUrl, i.Caption, i.IsMain, i.DisplayOrder)).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (data == null)
            return null;
        
        return new AttractionDetailDto(
            data.Id,
            data.Localized.Name,
            data.Localized.Description,
            data.Type,
            data.Location == null ? null : new LocationDto(data.Location.Latitude, data.Location.Longitude),
            data.HistoricalPeriods,
            data.Localized.LocationDescription,
            data.images,
            []);
    }

    public async Task<AttractionAdminDetailDto?> GetByIdForAdminAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var data = await Attractions.Where(a => a.Id == id)
            .Select(a => new {
                a.Id,
                Localized = a.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Name, lc.Description, lc.LocationDescription })
                    .FirstOrDefault()!,
                a.Type,
                a.Location,
                HistoricalPeriods = a.HistoricalPeriods.ToList(),
                a.IsFeatured,
                images = a.Images.Select(i => new ImageDto(i.Id, i.StorageKey, i.ImageUrl, i.Caption, i.IsMain, i.DisplayOrder)).ToList(),
                a.CreatedAt,
                a.CreatedBy,
                a.LastModifiedAt,
                a.LastModifiedBy
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (data == null)
            return null;
        
        return new AttractionAdminDetailDto(
            data.Id,
            data.Localized.Name,
            data.Localized.Description,
            data.Type,
            data.Location == null ? null : new LocationDto(data.Location.Latitude, data.Location.Longitude),
            data.HistoricalPeriods,
            data.Localized.LocationDescription,
            data.IsFeatured,
            data.images,
            data.CreatedAt,
            data.CreatedBy,
            data.LastModifiedAt,
            data.LastModifiedBy);
    }

    public async Task<List<AttractionListDto>> GetBySiteIdAsync(
        Guid siteId,
        AttractionType? type,
        string? searchTerm = null,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Attractions.Where(a => a.SiteId == siteId);
        if (type != null)
        {
            query = query.Where(a => a.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s =>
                s.LocalizedContents.Any(lc =>
                    lc.Language == language &&
                    EF.Functions.Like(lc.Name, $"%{searchTerm}%")));
        }
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderBy(a => a.IsFeatured)
            .Select(a => new AttractionListDto(
                a.Id,
                a.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                a.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Description)
                    .FirstOrDefault()!,
                a.Type,
                a.Location == null ? null : new LocationDto(a.Location.Latitude, a.Location.Longitude),
                a.MainImageUrl))
            .ToListAsync(cancellationToken);
        
        return items;
    }

    public async Task<List<AttractionLocalizedContentDto>> GetLocalizedContentsAsync(
        Guid attractionId,
        CancellationToken cancellationToken = default)
    {
        return await Attractions
            .Where(a => a.Id == attractionId)
            .SelectMany(a => a.LocalizedContents)
            .Select(lc => new AttractionLocalizedContentDto(
                lc.Id,
                lc.Language,
                lc.Name,
                lc.Description,
                lc.LocationDescription
            )).ToListAsync(cancellationToken);
    }

    public async Task<AttractionLocalizedContentDto?> GetLocalizedContentByIdAsync(
        Guid attractionId,
        Guid contentId,
        CancellationToken cancellationToken = default)
    {
        return await Attractions
            .Where(a => a.Id == attractionId)
            .SelectMany(a => a.LocalizedContents)
            .Where(lc => lc.Id == contentId)
            .Select(lc => new AttractionLocalizedContentDto(
                lc.Id,
                lc.Language,
                lc.Name,
                lc.Description,
                lc.LocationDescription))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<ImageDto>> GetImagesAsync(
        Guid attractionId,
        CancellationToken cancellationToken = default)
    {
        return await Attractions
            .Where(a => a.Id == attractionId)
            .SelectMany(a => a.Images)
            .Select(i => new ImageDto(i.Id, i.StorageKey, i.ImageUrl, i.Caption, i.IsMain, i.DisplayOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task<ImageDto?> GetImageByIdAsync(
        Guid attractionId,
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        return await Attractions
            .Where(a => a.Id == attractionId)
            .SelectMany(a => a.Images)
            .Where(i => i.Id == imageId)
            .Select(i => new ImageDto(i.Id, i.StorageKey, i.ImageUrl, i.Caption, i.IsMain, i.DisplayOrder))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<AttractionListDto>> GetAllAsync(
        string? searchTerm,
        AttractionType? type,
        PagingParameters paging,
        CancellationToken cancellationToken)
    {
        var query = Attractions;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s =>
                s.LocalizedContents.Any(lc =>
                    EF.Functions.Like(lc.Name, $"%{searchTerm}%")));   
        }

        if (type != null)
        {
            query = query.Where(a => a.Type == type);
        }
        
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.IsFeatured == true)
            .Skip(paging.Skip)
            .Take(paging.Take)
            .Select(a => new AttractionListDto(
                a.Id,
                a.LocalizedContents.Where(lc => lc.Language == LanguageCode.English)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                a.LocalizedContents.Where(lc => lc.Language == LanguageCode.English)
                    .Select(lc => lc.Description)
                    .FirstOrDefault()!,
                a.Type,
                a.Location == null ? null : new LocationDto(a.Location.Latitude, a.Location.Longitude),
                a.MainImageUrl
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<AttractionListDto>(
            items,
            totalCount,
            paging.Page,
            paging.PageSize);
    }

    public async Task<AdminAttractionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var dashboardData = await Attractions
            .GroupBy(_ => 1)
            .Select(g => new AdminAttractionDashboardDto(
                TotalAttractions: g.Count(),
                FeaturedAttractions: g.Count(s => s.IsFeatured)
            ))
            .FirstOrDefaultAsync(cancellationToken);
        
        return dashboardData ?? new AdminAttractionDashboardDto(0, 0);
    }

    public async Task<bool> AnyAsync(Guid attractionId, CancellationToken cancellationToken)
    {
        return await context.Attractions.AnyAsync(a => a.Id == attractionId, cancellationToken);
    }
}
