using Application.Common.Interfaces.QueryServices;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryServices;

internal class AttractionQueryService(
    ApplicationDbContext context) : IAttractionQueryService
{
    public async Task<AttractionDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Attractions
            .Where(a => a.Id == id)
            .Select(a => new AttractionDetailDto(
                a.Id,
                a.Name,
                a.Description,
                a.Type.ToString(),
                a.Location == null ? null : new LocationDto(a.Location.Latitude, a.Location.Longitude),
                a.HistoricalPeriod,
                a.LocationDescription,
                a.Images.Select(i => new ImageDto(i.Id, i.ImageUrl, i.Caption, i.IsMain, i.DisplayOrder)).ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<AttractionListDto>> GetBySiteIdAsync(
        Guid siteId,
        AttractionType type,
        PagingParameters paging,
        CancellationToken cancellationToken = default)
    {
        var query = context.Attractions
            .Where(a => a.SiteId == siteId && a.Type == type);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderBy(a => a.Name)
            .Skip(paging.Skip)
            .Take(paging.Take)
            .Select(a => new AttractionListDto(
                a.Id,
                a.Name,
                a.Description,
                a.Type.ToString(),
                a.Location == null ? null : new LocationDto(a.Location.Latitude, a.Location.Longitude),
                a.Images.First(i => i.IsMain).ImageUrl))
            .ToListAsync(cancellationToken);
        
        return new PagedResult<AttractionListDto>(items, totalCount, paging.PageNumber, paging.PageSize);
    }

    public async Task<AttractionAdminDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Attractions
            .Where(a => a.Id == id)
            .Select(a => new AttractionAdminDto(
                a.Id,
                a.Name,
                a.Description,
                a.Type.ToString(),
                a.Location == null ? null : new LocationDto(a.Location.Latitude, a.Location.Longitude),
                a.HistoricalPeriod,
                a.LocationDescription,
                a.Images.Select(i => new ImageDto(i.Id, i.ImageUrl, i.Caption, i.IsMain, i.DisplayOrder)).ToList(),
                a.CreatedAt,
                a.LastModified))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
