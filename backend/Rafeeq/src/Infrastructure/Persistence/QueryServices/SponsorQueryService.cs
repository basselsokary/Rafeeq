using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;
using Domain.Entities.SponsorAggregate;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryServices;

internal class SponsorQueryService(
    ApplicationDbContext context) : ISponsorQueryService
{
    public async Task<PagedResult<SponsorOfferListDto>> GetAllOffersAsync(
        SponsorFilters filters,
        PagingParameters paging,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var sponsorsQuery = ApplyFilters(context.Sponsors.AsNoTracking(), filters);
        var now = DateTime.UtcNow;

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
                o.ValidityPeriod.StartDate <= now && o.ValidityPeriod.EndDate >= now,
                o.IsActive &&
                o.ValidityPeriod.StartDate <= now &&
                o.ValidityPeriod.EndDate >= now &&
                (!o.MaxRedemptions.HasValue || o.RedemptionCount < o.MaxRedemptions.Value),
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
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SponsorListDto(
                s.Id,
                s.Title,
                s.Description.Substring(0, Math.Min(s.Description.Length, 100)) + (s.Description.Length > 100 ? "..." : string.Empty),
                s.Type.ToString(),
                s.Tier.ToString(),
                s.Location.Latitude,
                s.Location.Longitude,
                s.MainImageUrl,
                0,
                0,
                s.IsActive,
                s.Offers.Count(o => o.IsActive),
                null));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<SponsorListDto>(items, totalCount, paging.PageNumber, paging.PageSize);
    }

    public async Task<SponsorDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await context.Sponsors
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SponsorDetailDto(
                s.Id,
                s.Title,
                s.Description,
                s.Type.ToString(),
                s.Tier.ToString(),
                new LocationDto(s.Location.Latitude, s.Location.Longitude),
                new AddressDto(s.Address.Street, s.Address.City, s.Address.Region, s.Address.PostalCode, s.Address.ToString()),
                s.ContactPhone != null ? s.ContactPhone.ToString() : string.Empty,
                s.ContactEmail != null ? s.ContactEmail.ToString() : string.Empty,
                s.Website,
                0,
                0,
                s.Images
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new ImageDto(i.Id, i.ImageUrl, i.Caption, i.IsMain, i.DisplayOrder))
                    .ToList(),
                s.Offers
                    .Where(o => o.IsActive && o.ValidityPeriod.StartDate <= now && o.ValidityPeriod.EndDate >= now)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => new SponsorOfferDto(
                        o.Id,
                        s.Id,
                        s.Title,
                        o.Title,
                        o.Description,
                        o.DiscountAmount != null ? new MoneyDto(o.DiscountAmount.Amount, o.DiscountAmount.Currency, o.DiscountAmount.ToString()) : null,
                        o.DiscountPercentage,
                        new DateRangeDto(o.ValidityPeriod.StartDate, o.ValidityPeriod.EndDate, o.ValidityPeriod.DurationInDays),
                        o.ValidityPeriod.StartDate <= now && o.ValidityPeriod.EndDate >= now,
                        EF.Functions.DateDiffDay(now, o.ValidityPeriod.EndDate),
                        o.TermsAndConditions,
                        o.PromoCode,
                        o.MaxRedemptions,
                        o.RedemptionCount,
                        o.IsActive &&
                        o.ValidityPeriod.StartDate <= now &&
                        o.ValidityPeriod.EndDate >= now &&
                        (!o.MaxRedemptions.HasValue || o.RedemptionCount < o.MaxRedemptions.Value),
                        o.IsActive,
                        o.CreatedAt))
                    .ToList(),
                s.ContractStartDate,
                s.ContractEndDate,
                now >= s.ContractStartDate && now <= s.ContractEndDate,
                s.IsActive,
                0,
                s.TotalRedemptions,
                null))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<NearbySponsorDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        SponsorFilters filters,
        double radiusKm = 3,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

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
            .ThenByDescending(x => x.Sponsor.IsActive)
            .Take(count)
            .Select(x => new NearbySponsorDto(
                x.Sponsor.Id,
                x.Sponsor.Title,
                x.Sponsor.Type.ToString(),
                x.Sponsor.Location.Latitude,
                x.Sponsor.Location.Longitude,
                x.Distance,
                x.Sponsor.MainImageUrl,
                0,
                x.Sponsor.Offers.Any(o => o.IsActive && o.ValidityPeriod.StartDate <= now && o.ValidityPeriod.EndDate >= now),
                x.Sponsor.Offers.Count(o => o.IsActive && o.ValidityPeriod.StartDate <= now && o.ValidityPeriod.EndDate >= now)))
            .ToListAsync(cancellationToken);

        return candidates;
    }

    public async Task<SponsorOfferDto?> GetOfferByIdAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await context.Sponsors
            .AsNoTracking()
            .SelectMany(s => s.Offers)
            .Where(o => o.Id == offerId)
            .Select(o => new SponsorOfferDto(
                o.Id,
                o.Sponsor.Id,
                o.Sponsor.Title,
                o.Title,
                o.Description,
                o.DiscountAmount != null ? new MoneyDto(o.DiscountAmount.Amount, o.DiscountAmount.Currency, o.DiscountAmount.ToString()) : null,
                o.DiscountPercentage,
                new DateRangeDto(o.ValidityPeriod.StartDate, o.ValidityPeriod.EndDate, o.ValidityPeriod.DurationInDays),
                o.ValidityPeriod.StartDate <= now && o.ValidityPeriod.EndDate >= now,
                EF.Functions.DateDiffDay(now, o.ValidityPeriod.EndDate),
                o.TermsAndConditions,
                o.PromoCode,
                o.MaxRedemptions,
                o.RedemptionCount,
                o.IsActive &&
                o.ValidityPeriod.StartDate <= now &&
                o.ValidityPeriod.EndDate >= now &&
                (!o.MaxRedemptions.HasValue || o.RedemptionCount < o.MaxRedemptions.Value),
                o.IsActive,
                o.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<SponsorOfferDto>> GetOffersAsync(
        Guid sponsorId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

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
            .Select(o => new SponsorOfferDto(
                o.Id,
                o.Sponsor.Id,
                o.Sponsor.Title,
                o.Title,
                o.Description,
                o.DiscountAmount != null ? new MoneyDto(o.DiscountAmount.Amount, o.DiscountAmount.Currency, o.DiscountAmount.ToString()) : null,
                o.DiscountPercentage,
                new DateRangeDto(o.ValidityPeriod.StartDate, o.ValidityPeriod.EndDate, o.ValidityPeriod.DurationInDays),
                o.ValidityPeriod.StartDate <= now && o.ValidityPeriod.EndDate >= now,
                EF.Functions.DateDiffDay(now, o.ValidityPeriod.EndDate),
                o.TermsAndConditions,
                o.PromoCode,
                o.MaxRedemptions,
                o.RedemptionCount,
                o.IsActive &&
                o.ValidityPeriod.StartDate <= now &&
                o.ValidityPeriod.EndDate >= now &&
                (!o.MaxRedemptions.HasValue || o.RedemptionCount < o.MaxRedemptions.Value),
                o.IsActive,
                o.CreatedAt))
            .ToListAsync(cancellationToken);

        return entities;
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
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SponsorListDto(
                s.Id,
                s.Title,
                s.Description.Substring(0, Math.Min(s.Description.Length, 100)) + (s.Description.Length > 100 ? "..." : string.Empty),
                s.Type.ToString(),
                s.Tier.ToString(),
                s.Location.Latitude,
                s.Location.Longitude,
                s.MainImageUrl,
                0,
                0,
                s.IsActive,
                s.Offers.Count(o => o.IsActive),
                null));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

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

    public async Task<SponsorAdminDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await context.Sponsors
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SponsorAdminDetailDto(
                s.Id,
                s.Title,
                s.Description,
                s.Type.ToString(),
                s.Tier.ToString(),
                new LocationDto(s.Location.Latitude, s.Location.Longitude),
                new AddressDto(s.Address.Street, s.Address.City, s.Address.Region, s.Address.PostalCode, s.Address.ToString()),
                s.ContactPhone != null ? s.ContactPhone.ToString() : string.Empty,
                s.ContactEmail != null ? s.ContactEmail.ToString() : string.Empty,
                s.Website,
                0,
                0,
                s.ContractStartDate,
                s.ContractEndDate,
                now >= s.ContractStartDate && now <= s.ContractEndDate,
                s.IsActive,
                0,
                s.TotalRedemptions,
                null,
                s.CreatedAt,
                Guid.Empty,
                string.Empty,
                s.LastModifiedAt,
                null,
                null))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
