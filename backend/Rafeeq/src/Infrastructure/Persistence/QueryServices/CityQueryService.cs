using Microsoft.EntityFrameworkCore;
using Application.DTOs.Cities;
using Application.Common.Interfaces.QueryServices;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Common;

namespace Infrastructure.Persistence.QueryServices;

internal class CityQueryService(ApplicationDbContext context)
    : ICityQueryService
{
    public async Task<CityDetailDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.Cities
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CityDetailDto(
                c.Id,
                c.Name,
                c.Description,
                new(c.CenterLocation.Latitude, c.CenterLocation.Longitude),
                c.ImageUrl,
                c.TotalSites,
                c.DisplayOrder))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<CitySummaryDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await context.Cities
            .AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CitySummaryDto(c.Id, c.Name, c.ImageUrl))
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<CityListDto>> GetAsync(
        PagingParameters paging,
        CancellationToken cancellationToken = default)
    {
        var query = context.Cities
            .AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CityListDto(c.Id, c.Name, c.Description, c.ImageUrl, c.TotalSites));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<CityListDto>(
            items,
            totalCount,
            paging.PageNumber,
            paging.PageSize);
    }

}
