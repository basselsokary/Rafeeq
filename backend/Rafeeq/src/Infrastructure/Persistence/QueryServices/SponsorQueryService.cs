using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;
using Domain.Entities.SponsorAggregate;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Persistence.QueryServices;

internal class SponsorQueryService(ApplicationDbContext context) : ISponsorQueryService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<PagedResult<SponsorOfferListDto>> GetAllOffersAsync(
        SponsorFilters filters,
        PagingParameters paging,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var sponsorsQuery = ApplyFilters(context.Sponsors.AsNoTracking(), filters);

        var offersQuery = sponsorsQuery
            .SelectMany(s => s.Offers);

        if (activeOnly)
        {
            offersQuery = offersQuery.Where(o => o.IsActive);
        }

        var totalCount = await offersQuery.CountAsync(cancellationToken);
        var items = await offersQuery
            .OrderByDescending(o => o.DiscountPercentage)
            .Skip(paging.Skip)
            .Take(paging.Take)
            .Select(o => new SponsorOfferListDto(
                o.Id,
                o.Sponsor.Id,
                o.Sponsor.Title,
                o.Title,
                o.Description,
                o.DiscountAmount != null ? new MoneyDto(o.DiscountAmount.Amount, o.DiscountAmount.Currency, o.DiscountAmount.ToString()) : null,
                o.DiscountPercentage,
                new(o.ValidityPeriod.StartDate, o.ValidityPeriod.EndDate, o.ValidityPeriod.DurationInDays),
                o.IsValid(),
                o.CanBeRedeemed(),
                o.IsActive,
                o.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<SponsorOfferListDto>(
            items,
            totalCount,
            paging.PageNumber,
            paging.PageSize);
    }

    public async Task<PagedResult<SponsorListDto>> GetAsync(
        SponsorFilters filters,
        PagingParameters paging,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilters(context.Sponsors.AsNoTracking(), filters)
            .OrderByDescending(s => s.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var entities = await query
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        var items = entities.Select(Map<SponsorListDto>).ToList();
        return new PagedResult<SponsorListDto>(items, totalCount, paging.PageNumber, paging.PageSize);
    }

    public async Task<SponsorDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Sponsors
            .AsNoTracking()
            .Include(s => s.Offers)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return entity is null ? null : Map<SponsorDetailDto>(entity);
    }

    public async Task<List<NearbySponsorDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        SponsorFilters filters,
        double radiusKm = 3,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        var latDelta = radiusKm / 111d;
        var lonDelta = radiusKm / (111d * Math.Cos(latitude * Math.PI / 180d));

        var query = ApplyFilters(context.Sponsors.AsNoTracking(), filters)
            .Where(s =>
                s.Location.Latitude >= latitude - latDelta &&
                s.Location.Latitude <= latitude + latDelta &&
                s.Location.Longitude >= longitude - lonDelta &&
                s.Location.Longitude <= longitude + lonDelta);
        
        var candidates = await query
            .Select(s => new
            {
                Sponsor = s,
                Distance = HaversineKm(latitude, longitude, s.Location.Latitude, s.Location.Longitude)
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .Take(count)
            .ToListAsync(cancellationToken);
        return candidates.Select(x => Map<NearbySponsorDto>(x.Sponsor)).ToList();
    }

    public async Task<SponsorOfferDto?> GetOfferByIdAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        var offer = await context.Sponsors
            .AsNoTracking()
            .SelectMany(s => s.Offers)
            .FirstOrDefaultAsync(o => o.Id == offerId, cancellationToken);

        return offer is null ? null : Map<SponsorOfferDto>(offer);
    }

    public async Task<List<SponsorOfferDto>> GetOffersAsync(
        Guid sponsorId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = context.Sponsors
            .AsNoTracking()
            .Where(s => s.Id == sponsorId)
            .SelectMany(s => s.Offers);

        if (activeOnly)
        {
            query = query.Where(o => o.IsActive);
        }

        var entities = await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(Map<SponsorOfferDto>).ToList();
    }

    public async Task<PagedResult<SponsorListDto>> SearchAsync(
        string searchTerm,
        SponsorFilters filters,
        PagingParameters paging,
        CancellationToken cancellationToken = default)
    {
        var term = searchTerm.Trim();
        var query = ApplyFilters(context.Sponsors.AsNoTracking(), filters)
            .Where(s =>
                EF.Functions.Like(s.Title, $"%{term}%") ||
                EF.Functions.Like(s.Description, $"%{term}%"))
            .OrderByDescending(s => s.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var entities = await query
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        var items = entities.Select(Map<SponsorListDto>).ToList();
        return new PagedResult<SponsorListDto>(items, totalCount, paging.PageNumber, paging.PageSize);
    }

    private static IQueryable<Sponsor> ApplyFilters(IQueryable<Sponsor> query, SponsorFilters filters)
    {
        if (filters.Type.HasValue)
        {
            query = query.Where(s => filters.Type == s.Type);
        }

        if (filters.Tier.HasValue)
        {
            query = query.Where(s => filters.Tier == s.Tier);
        }

        if (filters.ActiveOnly.HasValue)
        {
            query = query.Where(s => s.IsActive == filters.ActiveOnly.Value);
        }

        return query;
    }

    private static TDto Map<TDto>(object source)
    {
        var json = JsonSerializer.Serialize(source, JsonOptions);
        return JsonSerializer.Deserialize<TDto>(json, JsonOptions)
               ?? throw new InvalidOperationException($"Unable to map to {typeof(TDto).Name}");
    }

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double r = 6371d;
        var dLat = (lat2 - lat1) * Math.PI / 180d;
        var dLon = (lon2 - lon1) * Math.PI / 180d;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180d) * Math.Cos(lat2 * Math.PI / 180d) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return r * c;
    }
}
