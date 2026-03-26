using Application.DTOs.Sites;
using Application.Common.Interfaces.QueryServices;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Common;
using System.Linq.Expressions;
using Domain.Entities.SiteAggregate;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity;
using Domain.Enums;
using Application.Common.Interfaces.Authentication;

namespace Infrastructure.Persistence.QueryServices;

internal class SiteQueryService(
    ApplicationDbContext context,
    IUserContext currentUser) : ISiteQueryService
{
    public async Task<SiteDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Sites.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SiteDetailDto(
                s.Id,
                s.Name,
                s.Description,
                s.Type.ToString(),
                s.Status.ToString(),
                new(s.Location.Latitude, s.Location.Longitude),
                new(s.Address.Street, s.Address.City, s.Address.Region, s.Address.PostalCode, s.Address.ToString()),
                s.ContactPhone,
                s.WebsiteUrl,
                s.AverageRating,
                s.TotalReviews,
                s.EntryFee != null ? new(s.EntryFee.Amount, s.EntryFee.Currency, s.EntryFee.ToString()) : null,
                s.Images.Select(i => new ImageDto(
                    i.Id,
                    i.ImageUrl,
                    i.Caption,
                    i.IsMain,
                    i.DisplayOrder)).Take(5).ToList(),
                s.OpeningHours.Select(oh => new OpeningHoursDto(
                    oh.DayOfWeek.ToString(),
                    oh.OpeningTime.StartTime.ToString(),
                    oh.OpeningTime.EndTime.ToString(),
                    oh.IsClosed)).ToList(),
                s.Facilities.Select(f => new FacilityDto(
                    f.Id,
                    f.Name,
                    f.Description,
                    f.IsAvailable)).ToList(),
                s.NearestTransportations.Select(nt => new NearestTransportationDto(
                    nt.Id,
                    nt.Type.ToString(),
                    nt.Name,
                    new(nt.Location.Latitude, nt.Location.Longitude),
                    nt.Address != null ? new(nt.Address.Street, nt.Address.City, nt.Address.Region, nt.Address.PostalCode, nt.Address.ToString()) : null,
                    nt.Description,
                    nt.IsOperational,
                    nt.HasAccessibility,
                    nt.OperatingHours != null ? new(nt.OperatingHours.StartTime.ToString(), nt.OperatingHours.EndTime.ToString(), nt.OperatingHours.DurationInMinutes) : null)).ToList(),
                s.IsFree,
                s.IsFeatured))
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<PagedResult<SiteListDto>> GetAsync(
        SiteFilters filters,
        PagingParameters paging,
        CancellationToken cancellationToken = default)
    {
        var query = context.Sites.AsNoTracking();
        query = ApplyFilters(query, filters);


        return await ToPagedResultAsync(
            query,
            paging,
            ConvertSiteToListDto(currentUser.Language),
            cancellationToken);
    }

    public Task<List<SiteListDto>> GetFeaturedAsync(
        int count = 10,
        Guid? city = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Sites
            .AsNoTracking()
            .Where(s => s.IsFeatured && s.Status == SiteStatus.Active);
        
        if (city.HasValue && city.Value != Guid.Empty)
        {
            query = query.Where(s => s.CityId == city.Value);
        }

        return query
            .OrderByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.TotalReviews)
            .Take(count)
            .Select(ConvertSiteToListDto(currentUser.Language))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LocalizedContentDto>> GetLocalizedContentsAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        return await context.Sites
            .AsNoTracking()
            .Where(s => s.Id == siteId)
            .SelectMany(s => s.LocalizedContents)
            .OrderBy(lc => lc.Language == currentUser.Language ? 0 : 1)
            .ThenBy(lc => lc.Language == LanguageCode.English ? 0 : 1)
            .Select(lc => new LocalizedContentDto(
                lc.Language.ToString(),
                lc.Name,
                lc.Description
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SiteListDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        SiteFilters filters,
        int radiusKm = 5,
        CancellationToken cancellationToken = default)
    {
        // coarse filter in SQL by bounding box, then exact distance in memory
        var latDelta = radiusKm / 111d;
        var lonDelta = radiusKm / (111d * Math.Cos(latitude * Math.PI / 180d));
        
        var query = context.Sites
            .AsNoTracking()
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
                Site = s,
                Distance = HaversineKm(
                    latitude,
                    longitude,
                    s.Location.Latitude,
                    s.Location.Longitude)
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .ThenByDescending(x => x.Site.AverageRating)
            .Take(100)
            .ToListAsync(cancellationToken);
        
        return candidates
            .Select(x => x.Site)
            .AsQueryable()
            .Select(ConvertSiteToListDto(currentUser.Language))
            .ToList();
    }

    public async Task<List<SiteListDto>> GetSimilarAsync(
        Guid siteId,
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        var seed = await context.Sites
            .AsNoTracking()
            .Where(s => s.Id == siteId)
            .Select(s => new { s.Id, s.Type, s.CityId })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (seed is null)
        {
            return [];
        }
        
        return await context.Sites
            .AsNoTracking()
            .Where(s =>
                s.Id != seed.Id &&
                s.Status == SiteStatus.Active &&
                (s.Type == seed.Type || s.CityId == seed.CityId))
            .OrderByDescending(s => s.Type == seed.Type)
            .ThenByDescending(s => s.CityId == seed.CityId)
            .ThenByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.TotalReviews)
            .Take(count)
            .Select(ConvertSiteToListDto(currentUser.Language))
            .ToListAsync(cancellationToken);
    }

    public Task<List<SiteMapMarkerDto>> GetWithinBoundsAsync(
        BoundingBox bounds,
        SiteFilters filters,
        int count = 20,
        CancellationToken cancellationToken = default)
    {
        var minLat = bounds.SouthLatitude;
        var maxLat = bounds.NorthLatitude;
        var minLng = bounds.WestLongitude;
        var maxLng = bounds.EastLongitude;
        
        var query = context.Sites
            .AsNoTracking()
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
                    .Where(lc => lc.Language == currentUser.Language || lc.Language == LanguageCode.English)
                    .Select(lc => lc.Name)
                    .First(),
                s.Type.ToString(),
                new LocationDto(s.Location.Latitude, s.Location.Longitude),
                s.AverageRating,
                s.MainImageUrl,
                s.IsFeatured
            ))
            .ToListAsync(cancellationToken);
    }

    public Task<PagedResult<SiteListDto>> SearchAsync(
        string searchTerm,
        SiteFilters filters,
        PagingParameters paging,
        CancellationToken cancellationToken = default)
    {
        var term = searchTerm.Trim();
        var query = context.Sites.AsNoTracking();
        query = ApplyFilters(query, filters);
        
        query = query.Where(s =>
            EF.Functions.Like(s.Name, $"%{term}%") ||
            EF.Functions.Like(s.Description, $"%{term}%") ||
            s.LocalizedContents.Any(lc =>
                EF.Functions.Like(lc.Name, $"%{term}%") ||
                EF.Functions.Like(lc.Description, $"%{term}%")));
        
        query = query.OrderByDescending(s => s.IsFeatured)
            .ThenByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.TotalReviews);
        
        return ToPagedResultAsync(
            query,
            paging,
            ConvertSiteToListDto(currentUser.Language),
            cancellationToken);
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
            paging.PageNumber,
            paging.PageSize);
    }

    private static IQueryable<Site> ApplyFilters(IQueryable<Site> query, SiteFilters filters)
    {
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

    private static Expression<Func<Site, SiteListDto>> ConvertSiteToListDto(LanguageCode language)
    {
        if (language != LanguageCode.English)
        {
            return s => new SiteListDto(
                s.Id,
                
                s.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .Select(lc => lc.Name)
                    .First(),
                s.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .Select(lc => lc.Description)
                    .First(),
                
                s.Type.ToString(),
                s.Status.ToString(),
                new(s.Location.Latitude, s.Location.Longitude),
                s.City.Name,
                s.MainImageUrl,
                s.AverageRating,
                s.TotalReviews,
                s.EntryFee != null ? s.EntryFee.Amount : 0,
                s.IsFree,
                s.IsFeatured);
        }

        return s => new SiteListDto(
            s.Id,
            s.Name,
            s.Description,
            s.Type.ToString(),
            s.Status.ToString(),
            new(s.Location.Latitude, s.Location.Longitude),
            s.City.Name,
            s.MainImageUrl,
            s.AverageRating,
            s.TotalReviews,
            s.EntryFee != null ? s.EntryFee.Amount : 0,
            s.IsFree,
            s.IsFeatured);
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

    public async Task<SiteAdminDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Sites.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SiteAdminDetailDto(
                s.Id,
                s.Name,
                s.Description,
                s.Type.ToString(),
                s.Status.ToString(),
                new(s.Location.Latitude, s.Location.Longitude),
                new(s.Address.Street, s.Address.City, s.Address.Region, s.Address.PostalCode, s.Address.ToString()),
                s.ContactPhone,
                s.WebsiteUrl,
                s.AverageRating,
                s.TotalReviews,
                s.EntryFee != null ? new(s.EntryFee.Amount, s.EntryFee.Currency, s.EntryFee.ToString()) : null,
                s.IsFree,
                s.IsFeatured,
                s.CreatedAt,
                s.LastModifiedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<PagedResult<SiteListDto>> GetByStatusAsync(SiteStatus status, SiteType type, PagingParameters paging, CancellationToken cancellationToken = default)
    {
        var query = context.Sites
            .AsNoTracking()
            .Where(s => s.Status == status && s.Type == type)
            .OrderByDescending(s => s.IsFeatured)
            .ThenByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.TotalReviews);

        return ToPagedResultAsync(
            query,
            paging,
            ConvertSiteToListDto(currentUser.Language),
            cancellationToken);
    }

}
