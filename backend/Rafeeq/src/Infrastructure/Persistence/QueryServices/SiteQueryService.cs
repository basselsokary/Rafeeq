using System.Linq.Expressions;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Application.DTOs.Common;
using Application.DTOs.Sites;
using Application.Extensions;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class SiteQueryService(
    ApplicationDbContext context,
    IDbContextFactory<ApplicationDbContext> dbContextFactory) : ISiteQueryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory = dbContextFactory;
    private IQueryable<Site> Sites => context.Sites.AsNoTracking();
    
    public async Task<AdminSiteDetailDto?> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var data = await Sites
            .Where(s => s.Id == id)
            .AsSplitQuery()
            .Select(s => new {
                s.Id,
                CityName = s.City.LocalizedContents.Where(lc => lc.Language == LanguageCode.English)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                Localized = s.LocalizedContents.Where(lc => lc.Language == LanguageCode.English)
                    .Select(lc => new {lc.Name, lc.Description, lc.Address, lc.EntryTicketNotes})
                    .FirstOrDefault()!,
                s.Type,
                s.Status,
                s.Location.Latitude, 
                s.Location.Longitude,
                s.ContactPhone,
                s.WebsiteUrl,
                s.MainImageUrl,
                s.AverageRating,
                s.TotalRating,
                s.EstimatedDurationMinutes,
                s.EntryTicket,
                s.IsFree,
                s.IsFeatured,
                s.IsHiddenGem,
                s.IsPopular,
                s.CreatedAt,
                s.CreatedBy,
                s.CreatedByName,
                s.LastModifiedAt,
                s.LastModifiedBy,
                s.LastModifiedByName
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (data is null)
            return null;
        
        var auditInfo = new AuditInfoDto(
            data.CreatedAt,
            data.CreatedBy,
            data.CreatedByName,
            data.LastModifiedAt,
            data.LastModifiedBy,
            data.LastModifiedByName);
        
        return data == null ? null : new AdminSiteDetailDto(
            data.Id,
            data.CityName,
            data.Localized.Name,
            data.Localized.Description,
            data.Type,
            data.Status,
            new(data.Latitude, data.Longitude),
            data.Localized.Address,
            data.ContactPhone,
            data.WebsiteUrl,
            data.MainImageUrl,
            data.AverageRating,
            data.TotalRating,
            data.EstimatedDurationMinutes,
            data.EntryTicket.ToDto(data.Localized.EntryTicketNotes),
            data.IsFree,
            data.IsFeatured,
            data.IsHiddenGem,
            data.IsPopular,
            auditInfo);
    }

    public Task<PagedResult<SiteListDto>> GetByStatusAsync(
        SiteStatus status,
        SiteType type,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Sites
            .Where(s => s.Status == status && s.Type == type)
            .OrderByDescending(s => s.IsFeatured)
            .ThenByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.TotalRating);

        return ToPagedResultAsync(
            query,
            paging,
            ConvertSiteToListDto(language),
            cancellationToken);
    }
    
    public async Task<SiteDetailDto?> GetByIdAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;
        
        var query = Sites.AsSplitQuery().Where(s => s.Id == id);
        return await GetSingleAsync(query, language, cancellationToken);
    }

    public async Task<SiteDetailDto?> GetByNameAsync(
        string name,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;
        
        name = name.Trim();
        var query = Sites.AsSplitQuery().Where(
                s => s.LocalizedContents.Any(
                    lc => (lc.Language == language || lc.Language == LanguageCode.English) && lc.Name == name));
        return await GetSingleAsync(query, language, cancellationToken);
    }

    private async Task<SiteDetailDto?> GetSingleAsync(
        IQueryable<Site> query,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var data = await query
            .Select(s => new {
                s.Id,
                CityName = s.City.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                Localized = s.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Name , lc.Description, lc.Address, lc.EntryTicketNotes })
                    .FirstOrDefault()!,
                s.Type,
                s.Status,
                LocationDto = new LocationDto(s.Location.Latitude, s.Location.Longitude),
                s.ContactPhone,
                s.WebsiteUrl,
                s.MainImageUrl,
                s.AverageRating,
                s.TotalRating,
                s.EstimatedDurationMinutes,
                s.EntryTicket,

                ImageDtos = s.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.DisplayOrder)
                    .Select(i => new ImageDto(
                        default,
                        i.StorageKey,
                        i.ImageUrl,
                        i.Caption,
                        i.IsMain,
                        i.DisplayOrder
                    ))
                    .ToList(),
                
                OpeningHourDtos = s.OpeningHours
                    .Select(oh => oh.ToDto())
                    .ToList(),
                
                FacilityTypes = s.Facilities.ToList(),
                
                NearestTransportation = s.NearestTransportations
                    .Select(t => new {
                        Id = (Guid)default,
                        t.Type,
                        Localized = t.LocalizedContents
                            .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                            .OrderBy(lc => lc.Language == language ? 0 : 1)
                            .Select(lc => new { lc.Name, lc.Description, lc.Address })
                            .FirstOrDefault()!,
                        LocationDto = new LocationDto(t.Location.Latitude, t.Location.Longitude),
                        t.DistanceKm,
                        t.IsOperational,
                        t.HasAccessibility,
                        TimeRange = t.OperatingHours
                    })
                    .ToList(),
                s.IsFree,
                s.IsFeatured,
                s.IsHiddenGem,
                s.IsPopular
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (data == null)
            return null;
        
        var site = new SiteDetailDto(
                data.Id,
                data.CityName,
                data.Localized.Name,
                data.Localized.Description,
                data.Type,
                data.Status,
                data.LocationDto,
                data.Localized.Address,
                data.ContactPhone,
                data.WebsiteUrl,
                data.MainImageUrl,
                data.AverageRating,
                data.TotalRating,
                data.EstimatedDurationMinutes,
                data.EntryTicket.ToDto(data.Localized.EntryTicketNotes),

                data.ImageDtos,
                data.OpeningHourDtos,
                data.FacilityTypes,
                data.NearestTransportation.Select(t => new NearestTransportationDto(
                    t.Id,
                    t.Type,
                    t.Localized.Name,
                    t.Localized.Description,
                    t.LocationDto,
                    t.Localized.Address,
                    t.DistanceKm,
                    t.IsOperational,
                    t.HasAccessibility,
                    t.TimeRange != null ? new TimeRangeDto(
                        t.TimeRange.StartTime,
                        t.TimeRange.EndTime,
                        t.TimeRange.DurationInMinutes
                    ) : null
                )).ToList(),

                data.IsFree,
                data.IsFeatured,
                data.IsHiddenGem,
                data.IsPopular,
                FacilityTypeDisplays: []
            );
        
        return site;
    }
    
    public async Task<PagedResult<SiteListDto>> GetAsync(
        SiteFilters filters,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Sites;
        query = ApplyFilters(query, filters)
            .OrderByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.IsPopular)
            .ThenByDescending(s => s.IsFeatured);

        return await ToPagedResultAsync(
            query,
            paging,
            ConvertSiteToListDto(language),
            cancellationToken);
    }

    public async Task<Dictionary<Guid, SiteListDto>> GetByIdsAsync(
        List<Guid> ids,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Sites
            .Where(s => s.LocalizedContents.Any(lc =>
                lc.Language == LanguageCode.English &&
                ids.Contains(lc.Id)));
        
        var sites = await query.Select(ConvertSiteToListDto(language)).ToListAsync(cancellationToken);
        return sites.ToDictionary(s => s.Id, s => s);
    }

    public async Task<List<SiteListDto>> GetByNamesAsync(
        List<string> names,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        names = names.Select(n => n.Trim()).ToList();
        var query = Sites;
        query = query
            .Where(s => s.LocalizedContents
                .Any(lc => (lc.Language == language || lc.Language == LanguageCode.English) && names.Contains(lc.Name)));
            
        return await query.Select(ConvertSiteToListDto(language)).ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, SiteListDto>> GetByEnglishNamesAsync(
        List<string> names,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        names = names.Select(n => n.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        var query = Sites
            .Where(s => s.LocalizedContents.Any(lc =>
                lc.Language == LanguageCode.English &&
                names.Contains(lc.Name)));
        
        return await GetSites(query, language, cancellationToken);
    }

    private static async Task<Dictionary<string, SiteListDto>> GetSites(
        IQueryable<Site> query,
        LanguageCode language,
        CancellationToken cancellationToken)
    {
        var sites = await query.Select(s => new
        {
            EnglishName = s.LocalizedContents
                .Where(lc => lc.Language == LanguageCode.English)
                .Select(lc => lc.Name)
                .FirstOrDefault(),

            Site = new SiteListDto(
            s.Id,

            s.City.LocalizedContents
                .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                .OrderBy(lc => lc.Language == language ? 0 : 1)
                .Select(lc => lc.Name)
                .FirstOrDefault()!,

            s.LocalizedContents
                .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                .OrderBy(lc => lc.Language == language ? 0 : 1)
                .Select(lc => lc.Name)
                .FirstOrDefault()!,

            s.Type,
            s.Status,
            new(s.Location.Latitude, s.Location.Longitude),
            s.MainImageUrl,
            s.AverageRating,
            s.TotalRating,
            s.IsFree,
            s.IsFeatured,
            s.IsHiddenGem)
        })
        .ToListAsync(cancellationToken);

        return sites
            .Where(x => !string.IsNullOrWhiteSpace(x.EnglishName))
            .ToDictionary(
                x => x.EnglishName!,
                x => x.Site,
                StringComparer.OrdinalIgnoreCase);
    }


    public Task<List<SiteListDto>> GetFeaturedAsync(
        int count = 10,
        Guid? city = null,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Sites
            .Where(s => s.IsFeatured && s.Status == SiteStatus.Active);
        
        if (city.HasValue && city.Value != Guid.Empty)
        {
            query = query.Where(s => s.CityId == city.Value);
        }

        return query
            .OrderByDescending(s => s.IsFeatured)
            .ThenByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.TotalRating)
            .Take(count)
            .Select(ConvertSiteToListDto(language))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SiteListDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        SiteFilters filters,
        int radiusKm = 40,
        int count = 20,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        // coarse filter in SQL by bounding box, then exact distance in memory
        var latDelta = radiusKm / 111d;
        var lonDelta = radiusKm / (111d * Math.Cos(latitude * Math.PI / 180d));
        
        var query = Sites
            .Where(s =>
                s.Location.Latitude >= latitude - latDelta &&
                s.Location.Latitude <= latitude + latDelta &&
                s.Location.Longitude >= longitude - lonDelta &&
                s.Location.Longitude <= longitude + lonDelta &&
                s.Status == SiteStatus.Active);
        
        if (filters is not null)
        {
            query = ApplyFilters(query, filters);
        }
        
        var candidates = await query
            .Select(s => new
            {
                Site = new SiteListDto(
                    s.Id,
                    s.City.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                        .OrderBy(lc => lc.Language == language ? 0 : 1)
                        .Select(lc => lc.Name)
                        .FirstOrDefault()!,
                    s.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                        .OrderBy(lc => lc.Language == language ? 0 : 1)
                        .Select(lc => lc.Name)
                        .FirstOrDefault()!,
                    s.Type,
                    s.Status,
                    new(s.Location.Latitude, s.Location.Longitude),
                    s.MainImageUrl,
                    s.AverageRating,
                    s.TotalRating,
                    s.IsFree,
                    s.IsFeatured,
                    s.IsHiddenGem),
                Distance = 6371 * 2 * Math.Asin(
                    Math.Sqrt(
                        Math.Pow(Math.Sin((latitude - s.Location.Latitude) * Math.PI / 180 / 2), 2) +
                        Math.Cos(latitude * Math.PI / 180) *
                        Math.Cos(s.Location.Latitude * Math.PI / 180) *
                        Math.Pow(Math.Sin((longitude - s.Location.Longitude) * Math.PI / 180 / 2), 2)
                    )
                )
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .ThenByDescending(x => x.Site.AverageRating)
            .Take(count) 
            .ToListAsync(cancellationToken);
        
        return candidates.Select(x => x.Site).ToList();
    }

    public async Task<List<SiteListDto>> GetSimilarAsync(
        Guid siteId,
        int count = 5,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var seed = await Sites
            .Where(s => s.Id == siteId)
            .Select(s => new { s.Id, s.Type, s.CityId })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (seed is null)
        {
            return [];
        }

        return await Sites
            .Where(s =>
                s.Id != seed.Id &&
                s.Status == SiteStatus.Active &&
                (s.Type == seed.Type || s.CityId == seed.CityId))
            .OrderByDescending(s => s.Type == seed.Type)
            .ThenByDescending(s => s.CityId == seed.CityId)
            .ThenByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.TotalRating)
            .Take(count)
            .Select(ConvertSiteToListDto(language))
            .ToListAsync(cancellationToken);
    }

    public Task<List<SiteMapMarkerDto>> GetWithinBoundsAsync(
        BoundingBox bounds,
        SiteFilters filters,
        int count = 20,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var minLat = bounds.SouthLatitude;
        var maxLat = bounds.NorthLatitude;
        var minLng = bounds.WestLongitude;
        var maxLng = bounds.EastLongitude;
        
        var query = Sites
            .Where(s =>
                s.Location.Latitude >= minLat &&
                s.Location.Latitude <= maxLat &&
                s.Location.Longitude >= minLng &&
                s.Location.Longitude <= maxLng &&
                s.Status == SiteStatus.Active);
        
        query = ApplyFilters(query, filters);
        
        return query
            .OrderByDescending(s => s.IsFeatured)
            .ThenByDescending(s => s.AverageRating)
            .Take(count)
            .Select(s => new SiteMapMarkerDto(
                s.Id,
                s.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                s.Type,
                new LocationDto(s.Location.Latitude, s.Location.Longitude),
                s.AverageRating,
                s.MainImageUrl,
                s.IsFeatured
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SiteSummaryDto>> GetMustVisitAsync(
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        return await db.Sites.AsNoTracking()
            .Where(s => s.Status == SiteStatus.Active)
            .Where(s => 
                s.IsFeatured || 
                s.AverageRating >= 4.5)
            .OrderByDescending(s => s.IsFeatured)
            .ThenByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.IsPopular)
            .Take(count)
            .Select(s => new SiteSummaryDto(
                s.Id,
                s.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                s.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Description)
                    .FirstOrDefault()!,
                s.City.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                s.Type,
                s.MainImageUrl,
                s.AverageRating,
                s.TotalRating,
                new LocationDto(s.Location.Latitude, s.Location.Longitude),
                s.IsFree,
                "Must Visit")
            )
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SiteSummaryDto>> GetHiddenGemsAsync(
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        return await db.Sites.AsNoTracking()
            .Where(s => s.Status == SiteStatus.Active)
            .Where(s => s.IsHiddenGem ||
                s.AverageRating >= 4.0 ||
                s.TotalRating >= 5) // At least some ratings
            .OrderByDescending(s => s.IsHiddenGem)
            .ThenBy(s => s.AverageRating)
            .Take(count)
            .Select(s => new SiteSummaryDto(
                s.Id,
                s.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                s.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Description)
                    .FirstOrDefault()!,
                s.City.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                s.Type,
                s.MainImageUrl,
                s.AverageRating,
                s.TotalRating,
                new LocationDto(s.Location.Latitude, s.Location.Longitude),
                s.IsFree,
                "Hidden Gem")
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SiteSummaryDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm = 40,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var latDelta = radiusKm / 111.0;
        var lonDelta = radiusKm / (111.0 * Math.Cos(latitude * Math.PI / 180.0));

        var minLat = latitude - latDelta;
        var maxLat = latitude + latDelta;
        var minLon = longitude - lonDelta;
        var maxLon = longitude + lonDelta;

        var sites = await db.Sites.AsNoTracking()
            .Where(a => a.Status == SiteStatus.Active)
            .Where(s => 
                s.Location.Latitude >= minLat &&
                s.Location.Latitude <= maxLat &&
                s.Location.Longitude >= minLon &&
                s.Location.Longitude <= maxLon)
            .Select(s => new
            {
                Site = s,
                Distance = 6371 * 2 * Math.Asin(
                    Math.Sqrt(
                        Math.Pow(Math.Sin((latitude - s.Location.Latitude) * Math.PI / 180 / 2), 2) +
                        Math.Cos(latitude * Math.PI / 180) *
                        Math.Cos(s.Location.Latitude * Math.PI / 180) *
                        Math.Pow(Math.Sin((longitude - s.Location.Longitude) * Math.PI / 180 / 2), 2)
                    )
                )
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .Take(count)
            .Select(s => new SiteSummaryDto(
                s.Site.Id,
                s.Site.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                s.Site.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Description)
                    .FirstOrDefault()!,
                s.Site.City.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                s.Site.Type,
                s.Site.MainImageUrl,
                s.Site.AverageRating,
                s.Site.TotalRating,
                new LocationDto(s.Site.Location.Latitude, s.Site.Location.Longitude),
                s.Site.IsFree,
                "Near You")
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return sites;
    }

    public async Task<List<SiteMapMarkerDto>> GetNearbyMarkerAsync(
        double latitude,
        double longitude,
        int radiusKm = 30,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var sites = Sites
            .Where(a => a.Status == SiteStatus.Active)
            .Select(s => new
            {
                Site = s,
                Distance = 6371 * 2 * Math.Asin(
                    Math.Sqrt(
                        Math.Pow(Math.Sin((latitude - s.Location.Latitude) * Math.PI / 180 / 2), 2) +
                        Math.Cos(latitude * Math.PI / 180) *
                        Math.Cos(s.Location.Latitude * Math.PI / 180) *
                        Math.Pow(Math.Sin((longitude - s.Location.Longitude) * Math.PI / 180 / 2), 2)
                    )
                )
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance);
        
        return await sites
            .OrderByDescending(s => s.Site.IsFeatured)
            .ThenByDescending(s => s.Site.AverageRating)
            .Take(count)
            .Select(s => new SiteMapMarkerDto(
                s.Site.Id,
                s.Site.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                s.Site.Type,
                new LocationDto(s.Site.Location.Latitude, s.Site.Location.Longitude),
                s.Site.AverageRating,
                s.Site.MainImageUrl,
                s.Site.IsFeatured
            ))
            .ToListAsync(cancellationToken);
    }

    public Task<PagedResult<SiteListDto>> SearchAsync(
        string searchTerm,
        SiteFilters filters,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var term = searchTerm.Trim();
        var query = Sites;
        query = ApplyFilters(query, filters);

        query = query.Where(s =>
            s.LocalizedContents.Any(lc =>
                lc.Language == language &&
                EF.Functions.Like(lc.Name, $"%{term}%")));
        
        query = query.OrderByDescending(s => s.IsFeatured)
            .ThenByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.TotalRating);
        
        return ToPagedResultAsync(
            query,
            paging,
            ConvertSiteToListDto(language),
            cancellationToken);
    }

    public Task<List<SiteLookupDto>> SearchAsync(
        string searchTerm,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        var term = searchTerm.Trim();
        var query = Sites.Where(s =>
            s.LocalizedContents.Any(lc =>
                lc.Language == LanguageCode.English &&
                lc.Name.Contains(term)));
        
        return query.Take(count)
            .OrderByDescending(s => s.AverageRating)
            .Select(s => new SiteLookupDto(
                s.Id,
                s.LocalizedContents.Where(lc => lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == LanguageCode.English ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AdminSiteLocalizedContentDto>> GetLocalizedContentsAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        return await Sites
            .Where(s => s.Id == siteId)
            .SelectMany(s => s.LocalizedContents)
            .Select(lc => new AdminSiteLocalizedContentDto(
                lc.Id,
                lc.Language,
                lc.Name,
                lc.Description,
                lc.Address,
                lc.EntryTicketNotes,
                new(
                    lc.CreatedAt,
                    lc.CreatedBy,
                    lc.CreatedByName,
                    lc.LastModifiedAt,
                    lc.LastModifiedBy,
                    lc.LastModifiedByName)
            )).ToListAsync(cancellationToken);
    }

    public async Task<AdminSiteLocalizedContentDto?> GetLocalizedContentByIdAsync(
        Guid siteId,
        Guid contentId,
        CancellationToken cancellationToken = default)
    {
        return await Sites
            .Where(s => s.Id == siteId)
            .SelectMany(s => s.LocalizedContents)
            .Where(lc => lc.Id == contentId)
            .Select(lc => new AdminSiteLocalizedContentDto(
                lc.Id,
                lc.Language,
                lc.Name,
                lc.Description,
                lc.Address,
                lc.EntryTicketNotes,
                new AuditInfoDto(
                    lc.CreatedAt,
                    lc.CreatedBy,
                    lc.CreatedByName,
                    lc.LastModifiedAt,
                    lc.LastModifiedBy,
                    lc.LastModifiedByName)
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<AdminSiteFacilityDto>> GetFacilitiesAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        var facilities = await Sites
            .Where(s => s.Id == siteId)
            .Select(s => s.Facilities)
            .FirstOrDefaultAsync(cancellationToken);

        if (facilities == null)
            return new List<AdminSiteFacilityDto>();

        return facilities
            .Select(f => new AdminSiteFacilityDto(f, f.ToString()))
            .ToList();
    }

    public async Task<List<AdminSiteNearestTransportationDto>> GetNearestTransportationAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        var data = await context.NearestTransportations.AsNoTracking()
            .Where(t => t.SiteId == siteId)
            .Select(t => new {
                t.Id,
                t.Type,
                LocationDto = new LocationDto(t.Location.Latitude, t.Location.Longitude),
                t.DistanceKm,
                t.IsOperational,
                t.HasAccessibility,
                t.OperatingHours,
                LocalizedContents = t.LocalizedContents.Select(lc => new AdminSiteNearestTransportationLocalizedContentDto(
                    lc.Language,
                    lc.Name,
                    lc.Description,
                    lc.Address,
                    new AuditInfoDto(
                        lc.CreatedAt,
                        lc.CreatedBy,
                        lc.CreatedByName,
                        lc.LastModifiedAt,
                        lc.LastModifiedBy,
                        lc.LastModifiedByName)
                )).ToList(),
                AuditInfo = new AuditInfoDto(
                    t.CreatedAt,
                    t.CreatedBy,
                    t.CreatedByName,
                    t.LastModifiedAt,
                    t.LastModifiedBy,
                    t.LastModifiedByName)
            }).ToListAsync(cancellationToken);
        
        return data.Select(t => new AdminSiteNearestTransportationDto(
            t.Id,
            t.Type,
            t.LocationDto,
            t.DistanceKm,
            t.IsOperational,
            t.HasAccessibility,
            t.OperatingHours != null ? new TimeRangeDto(
                t.OperatingHours.StartTime,
                t.OperatingHours.EndTime,
                t.OperatingHours.DurationInMinutes
            ) : null,
            t.LocalizedContents,
            t.AuditInfo
        )).ToList();
    }

    public async Task<AdminSiteNearestTransportationDto?> GetNearestTransportationByIdAsync(
        Guid siteId,
        Guid transportationId,
        CancellationToken cancellationToken = default)
    {
        var data = await context.NearestTransportations.AsNoTracking()
            .Where(t => t.SiteId == siteId && t.Id == transportationId)
            .Select(t => new {
                t.Id,
                t.Type,
                LocationDto = new LocationDto(t.Location.Latitude, t.Location.Longitude),
                t.DistanceKm,
                t.IsOperational,
                t.HasAccessibility,
                t.OperatingHours,
                LocalizedContents = t.LocalizedContents.Select(lc => new AdminSiteNearestTransportationLocalizedContentDto(
                    lc.Language,
                    lc.Name,
                    lc.Description,
                    lc.Address,
                    new AuditInfoDto(
                        lc.CreatedAt,
                        lc.CreatedBy,
                        lc.CreatedByName,
                        lc.LastModifiedAt,
                        lc.LastModifiedBy,
                        lc.LastModifiedByName)
                )).ToList(),
                AuditInfo = new AuditInfoDto(
                    t.CreatedAt,
                    t.CreatedBy,
                    t.CreatedByName,
                    t.LastModifiedAt,
                    t.LastModifiedBy,
                    t.LastModifiedByName)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (data is null)
            return null;

        return new AdminSiteNearestTransportationDto(
            data.Id,
            data.Type,
            data.LocationDto,
            data.DistanceKm,
            data.IsOperational,
            data.HasAccessibility,
            data.OperatingHours != null
                ? new TimeRangeDto(
                    data.OperatingHours.StartTime,
                    data.OperatingHours.EndTime,
                    data.OperatingHours.DurationInMinutes)
                : null,
            data.LocalizedContents,
            data.AuditInfo);
    }

    public Task<AdminSiteNearestTransportationLocalizedContentDto?> GetNearestTransportationLocalizedContentByIdAsync(
        Guid siteId,
        Guid transportationId,
        Guid contentId,
        CancellationToken cancellationToken = default)
    {
        return context.NearestTransportations.AsNoTracking()
            .Where(t => t.SiteId == siteId && t.Id == transportationId)
            .SelectMany(t => t.LocalizedContents)
            .Where(lc => lc.Id == contentId)
            .Select(lc => new AdminSiteNearestTransportationLocalizedContentDto(
                lc.Language,
                lc.Name,
                lc.Description,
                lc.Address,
                new AuditInfoDto(
                    lc.CreatedAt,
                    lc.CreatedBy,
                    lc.CreatedByName,
                    lc.LastModifiedAt,
                    lc.LastModifiedBy,
                    lc.LastModifiedByName)
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<ImageDto>> GetImagesAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        return Sites
            .Where(s => s.Id == siteId)
            .SelectMany(s => s.Images)
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new ImageDto(
                i.Id,
                i.StorageKey,
                i.ImageUrl,
                i.Caption,
                i.IsMain,
                i.DisplayOrder
            )).ToListAsync(cancellationToken);
    }

    public Task<ImageDto?> GetImageByIdAsync(
        Guid siteId,
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        return Sites
            .Where(s => s.Id == siteId)
            .SelectMany(s => s.Images)
            .Where(i => i.Id == imageId)
            .Select(i => new ImageDto(
                i.Id,
                i.StorageKey,
                i.ImageUrl,
                i.Caption,
                i.IsMain,
                i.DisplayOrder
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        IQueryable<Site> query,
        PagingParameters paging,
        Expression<Func<Site, T>> selector,
        CancellationToken cancellationToken)
        where T : class
    {
        var queryPaging = query.Select(selector);

        var totalCount = await queryPaging.CountAsync(cancellationToken);
        var items = await queryPaging
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(
            items,
            totalCount,
            paging.Page,
            paging.PageSize);
    }

    private static IQueryable<Site> ApplyFilters(IQueryable<Site> query, SiteFilters filters)
    {
        if (filters is null)
            return query;

        if (filters.Type is not null)
        {
            query = query.Where(s => s.Type == filters.Type);
        }

        if (filters.City.HasValue && filters.City.Value != Guid.Empty)
        {
            query = query.Where(s => s.CityId == filters.City.Value);
        }

        if (filters.IsFree.HasValue)
        {
            query = query.Where(s => s.IsFree == filters.IsFree);
        }

        if (filters.MaxRating.HasValue)
        {
            query = query.Where(s => s.AverageRating <= filters.MaxRating);
        }
        
        if (filters.MinRating.HasValue)
        {
            query = query.Where(s => s.AverageRating >= filters.MinRating);
        }

        return query;
    }

    private Expression<Func<Site, SiteListDto>> ConvertSiteToListDto(LanguageCode language)
    {
        return s => new SiteListDto(
            s.Id,
            s.City.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                .OrderBy(lc => lc.Language == language ? 0 : 1)
                .Select(lc => lc.Name)
                .FirstOrDefault()!,
            s.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                .OrderBy(lc => lc.Language == language ? 0 : 1)
                .Select(lc => lc.Name)
                .FirstOrDefault()!,
            s.Type,
            s.Status,
            new(s.Location.Latitude, s.Location.Longitude),
            s.MainImageUrl,
            s.AverageRating,
            s.TotalRating,
            s.IsFree,
            s.IsFeatured,
            s.IsHiddenGem);
    }

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371d;
        var dLat = (lat2 - lat1) * Math.PI / 180d;
        var dLon = (lon2 - lon1) * Math.PI / 180d;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180d) * Math.Cos(lat2 * Math.PI / 180d) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    public async Task<List<AdminSiteOpeningHourDto>> GetOpeningHoursAsync(Guid siteId, CancellationToken cancellationToken)
    {
        return await Sites.Where(s => s.Id == siteId)
            .SelectMany(s => s.OpeningHours)
            .Select(oh => new AdminSiteOpeningHourDto(
                oh.Day,
                oh.OpeningTime == null? null : oh.OpeningTime.StartTime,
                oh.OpeningTime == null? null : oh.OpeningTime.EndTime,
                oh.IsClosed))
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminSiteDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var dashboardDto = await Sites
            .GroupBy(_ => 1)
            .Select(g => new AdminSiteDashboardDto(
                TotalSites: g.Count(),
                ActiveSites: g.Count(s => s.Status == SiteStatus.Active),
                FeaturedSites: g.Count(s => s.IsFeatured),
                HiddenGemSites: g.Count(s => s.IsHiddenGem),
                AverageRating: g.Average(s => s.AverageRating),
                TotalRating: g.Sum(s => s.TotalRating)
            ))
            .FirstOrDefaultAsync(cancellationToken);
        
        return dashboardDto ?? new AdminSiteDashboardDto(0, 0, 0, 0, 0, 0);
    }

    public Task<bool> AnyAsync(Guid siteId, CancellationToken cancellationToken)
    {
        return context.Sites.AnyAsync(s => s.Id == siteId, cancellationToken);
    }
}
