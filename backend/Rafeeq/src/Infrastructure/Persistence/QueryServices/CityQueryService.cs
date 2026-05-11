using Microsoft.EntityFrameworkCore;
using Application.DTOs.Cities;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Infrastructure.Persistence.ApplicationContext;
using Domain.Enums;
using Domain.Entities.CityAggregate;
using Application.DTOs.Admins;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class CityQueryService(
    ApplicationDbContext context) : ICityQueryService
{
    private IQueryable<City> Cities => context.Cities.AsNoTracking();
    
    public async Task<CityDetailDto?> GetByIdAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        return await Cities
            .Where(c => c.Id == id)
            .Select(c => new CityDetailDto(
                c.Id,
                c.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                c.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Description)
                    .FirstOrDefault()!,
                new(c.CenterLocation.Latitude, c.CenterLocation.Longitude),
                c.ImageUrl,
                c.TotalSites,
                c.DisplayOrder))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CityAdminDetailDto?> GetByIdForAdminAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var data = await Cities
            .Where(c => c.Id == id)
            .Select(c => new {
                c.Id,
                Localized = c.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Name, lc.Description })
                    .FirstOrDefault()!,
                Location = new LocationDto(c.CenterLocation.Latitude, c.CenterLocation.Longitude),
                c.ImageUrl,
                c.TotalSites,
                c.DisplayOrder,
                c.CreatedAt,
                c.LastModifiedAt
                }
            ).FirstOrDefaultAsync(cancellationToken);
        
        return data == null ? null : new CityAdminDetailDto(
            data.Id,
            data.Localized.Name,
            data.Localized.Description,
            data.Location,
            data.ImageUrl,
            data.TotalSites,
            data.DisplayOrder,
            data.CreatedAt,
            data.LastModifiedAt);
    }

    public async Task<List<CitySummaryDto>> GetSummariesAsync(
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        return await Cities
            .OrderBy(c => c.DisplayOrder)
            .ThenByDescending(c => c.TotalSites)
            .Select(c => new CitySummaryDto(
                c.Id,
                c.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                c.ImageUrl))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CityListDto>> GetAsync(
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Cities
            .OrderBy(c => c.DisplayOrder)
            .ThenByDescending(c => c.TotalSites)
            .Select(c => new CityListDto(
                c.Id,
                c.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                c.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Description)
                    .FirstOrDefault()!,
                new LocationDto(c.CenterLocation.Latitude, c.CenterLocation.Longitude),
                c.ImageUrl,
                c.TotalSites));
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<CityLocalizedContentDto>> GetLocalizedContentsAsync(
        Guid cityId,
        CancellationToken cancellationToken = default)
    {
        return await Cities
            .Where(c => c.Id == cityId)
            .SelectMany(c => c.LocalizedContents)
            .Select(lc => new CityLocalizedContentDto(
                lc.Id,
                lc.Language,
                lc.Name,
                lc.Description
            )).ToListAsync(cancellationToken);
    }

    public async Task<CityLocalizedContentDto?> GetLocalizedContentByIdAsync(
        Guid cityId, Guid contentId, CancellationToken cancellationToken)
    {
        return await Cities
            .Where(c => c.Id == cityId)
            .SelectMany(c => c.LocalizedContents)
            .Where(lc => lc.Id == contentId)
            .Select(lc => new CityLocalizedContentDto(
                lc.Id,
                lc.Language,
                lc.Name,
                lc.Description
            )).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AdminCityDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var dashboardData = await Cities
            .GroupBy(_ => 1)
            .Select(g => new AdminCityDashboardDto(
                TotalCities: g.Count(),
                TotalSites: g.Sum(c => c.TotalSites)
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return dashboardData ?? new AdminCityDashboardDto(0, 0);
    }
}
