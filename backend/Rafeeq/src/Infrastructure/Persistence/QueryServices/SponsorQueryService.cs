using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class SponsorQueryService(
    ApplicationDbContext context,
    IDbContextFactory<ApplicationDbContext> dbContextFactory) : ISponsorQueryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory = dbContextFactory;
    private IQueryable<Sponsor> Sponsors => context.Sponsors.AsNoTracking();

    public async Task<AdminSponsorDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var data = await Sponsors
            .Where(s => s.Id == id)
            .Select(s => new {
                s.Id,
                Localized = s.LocalizedContents.Where(lc => lc.Language == LanguageCode.English)
                    .Select(lc => new {lc.Title, lc.Description, lc.Address})
                    .FirstOrDefault()!,
                s.Type,
                s.Tier,
                s.Location,
                s.ContactPhone,
                s.ContactEmail,
                s.WebsiteUrl,
                s.ContractDate,
                s.MainImageUrl,
                s.Status,
                s.TotalRedemptions,
                s.CreatedAt,
                s.LastModifiedAt,
                s.LastModifiedBy,
                s.LastModifiedByName})
            .FirstOrDefaultAsync(cancellationToken);

        if (data == null)
            return null;
        
        return new AdminSponsorDetailDto(
                data.Id,
                data.Localized.Title,
                data.Localized.Description,
                data.Type,
                data.Tier,
                new LocationDto(data.Location.Latitude, data.Location.Longitude),
                data.Localized.Address,
                data.ContactPhone ?? string.Empty,
                data.ContactEmail ?? string.Empty,
                data.WebsiteUrl,
                data.MainImageUrl,
                new(data.ContractDate.StartDate, data.ContractDate.EndDate, data.ContractDate.DurationInDays),
                data.ContractDate.IsWithinRange(now),
                data.Status,
                data.TotalRedemptions,
                data.CreatedAt,
                Guid.Empty,
                string.Empty,
                data.LastModifiedAt,
                data.LastModifiedBy,
                data.LastModifiedByName);
    }

    public async Task<PagedResult<SponsorOfferListDto>> GetAllOffersAsync(
        SponsorFilters filters,
        PagingParameters paging,
        bool activeOnly = true,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var sponsorsQuery = ApplyFilters(Sponsors, filters);

        var offersQuery = sponsorsQuery
            .SelectMany(s => s.Offers);

        if (activeOnly)
        {
            offersQuery = offersQuery.Where(o => o.IsActive);
        }

        var now = DateTime.UtcNow;
        var totalCount = await offersQuery.CountAsync(cancellationToken);
        var items = await offersQuery
            .Select(o => new
            {
                Offer = o,
                SponsorLocalized = o.Sponsor.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Title })
                    .FirstOrDefault()!,
                OfferLocalized = o.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Title, lc.Description })
                    .FirstOrDefault()!
            })
            .OrderByDescending(x => x.Offer.DiscountPercentage)
            .Skip(paging.Skip)
            .Take(paging.Take)
            .Select(x => new SponsorOfferListDto(
                x.Offer.Id,
                x.Offer.Sponsor.Id,
                x.SponsorLocalized.Title,
                x.OfferLocalized.Title,
                x.OfferLocalized.Description,
                x.Offer.DiscountAmount != null ? new MoneyDto(x.Offer.DiscountAmount.Amount, x.Offer.DiscountAmount.Currency, x.Offer.DiscountAmount.ToString()) : null,
                x.Offer.DiscountPercentage,
                new(x.Offer.ValidityPeriod.StartDate, x.Offer.ValidityPeriod.EndDate, x.Offer.ValidityPeriod.DurationInDays),
                x.Offer.ValidityPeriod.StartDate <= now && x.Offer.ValidityPeriod.EndDate >= now,
                x.Offer.IsActive &&
                x.Offer.ValidityPeriod.StartDate <= now &&
                x.Offer.ValidityPeriod.EndDate >= now &&
                (!x.Offer.MaxRedemptions.HasValue || x.Offer.RedemptionCount < x.Offer.MaxRedemptions.Value),
                x.Offer.IsActive,
                x.Offer.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<SponsorOfferListDto>(
            items,
            totalCount,
            paging.Page,
            paging.PageSize);
    }

    public async Task<PagedResult<SponsorListDto>> GetAsync(
        SponsorFilters filters,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var query = ApplyFilters(Sponsors, filters)
            .Select(s => new
            {
                Sponsor = s,
                Localized = s.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Title, lc.Description })
                    .FirstOrDefault()!
            })
            .OrderByDescending(x => x.Sponsor.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(paging.Skip)
            .Take(paging.Take)
            .Select(x => new SponsorListDto(
                x.Sponsor.Id,
                x.Localized.Title,
                x.Sponsor.Type,
                x.Sponsor.Location.Latitude,
                x.Sponsor.Location.Longitude,
                x.Sponsor.MainImageUrl,
                x.Sponsor.ContractDate.StartDate <= now && x.Sponsor.ContractDate.EndDate >= now,
                x.Sponsor.Status,
                x.Sponsor.Offers.Count(o => o.IsActive),
                null))
            .ToListAsync(cancellationToken);

        return new PagedResult<SponsorListDto>(items, totalCount, paging.Page, paging.PageSize);
    }

    public async Task<SponsorDetailDto?> GetByIdAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await Sponsors
            .AsSplitQuery()
            .Where(s => s.Id == id)
            .Select(s => new
            {
                Sponsor = s,
                Localized = s.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Title, lc.Description, Address = lc.Address })
                    .FirstOrDefault()!
            })
            .Select(x => new SponsorDetailDto(
                x.Sponsor.Id,
                x.Localized.Title,
                x.Localized.Description,
                x.Sponsor.Type,
                new LocationDto(x.Sponsor.Location.Latitude, x.Sponsor.Location.Longitude),
                x.Localized.Address,
                x.Sponsor.ContactPhone,
                x.Sponsor.ContactEmail,
                x.Sponsor.WebsiteUrl,
                x.Sponsor.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.DisplayOrder)
                    .Select(i => new ImageDto(i.Id, i.StorageKey, i.ImageUrl, i.Caption, i.IsMain, i.DisplayOrder))
                    .ToList(),
                x.Sponsor.Offers
                    .Where(o => o.IsActive && o.ValidityPeriod.StartDate <= now && o.ValidityPeriod.EndDate >= now)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => new
                    {
                        Offer = o,
                        OfferLocalized = o.LocalizedContents
                            .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                            .OrderBy(lc => lc.Language == language ? 0 : 1)
                            .Select(lc => new { lc.Title, lc.Description, lc.TermsAndConditions })
                            .FirstOrDefault()!
                    })
                    .Select(y => new SponsorOfferDto(
                        y.Offer.Id,
                        x.Sponsor.Id,
                        x.Localized!.Title,
                        y.OfferLocalized.Title,
                        y.OfferLocalized.Description,
                        y.Offer.DiscountAmount != null ? new MoneyDto(y.Offer.DiscountAmount.Amount, y.Offer.DiscountAmount.Currency, y.Offer.DiscountAmount.ToString()) : null,
                        y.Offer.DiscountPercentage,
                        new DateRangeDto(y.Offer.ValidityPeriod.StartDate, y.Offer.ValidityPeriod.EndDate, y.Offer.ValidityPeriod.DurationInDays),
                        y.Offer.ValidityPeriod.StartDate <= now && y.Offer.ValidityPeriod.EndDate >= now,
                        EF.Functions.DateDiffDay(now, y.Offer.ValidityPeriod.EndDate),
                        y.OfferLocalized.TermsAndConditions,
                        y.Offer.PromoCode,
                        y.Offer.MaxRedemptions,
                        y.Offer.RedemptionCount,
                        y.Offer.IsActive &&
                        y.Offer.ValidityPeriod.StartDate <= now &&
                        y.Offer.ValidityPeriod.EndDate >= now &&
                        (!y.Offer.MaxRedemptions.HasValue || y.Offer.RedemptionCount < y.Offer.MaxRedemptions.Value),
                        y.Offer.IsActive,
                        y.Offer.CreatedAt))
                    .ToList(),
                new(x.Sponsor.ContractDate.StartDate, x.Sponsor.ContractDate.EndDate, x.Sponsor.ContractDate.DurationInDays),
                x.Sponsor.ContractDate.IsWithinRange(now),
                x.Sponsor.Status,
                x.Sponsor.TotalRedemptions))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<NearbySponsorDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        SponsorFilters filters,
        double radiusKm = 30,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var latDelta = radiusKm / 111d;
        var lonDelta = radiusKm / (111d * Math.Cos(latitude * Math.PI / 180d));

        var query = ApplyFilters(Sponsors, filters)
            .Where(s =>
                s.Location.Latitude >= latitude - latDelta &&
                s.Location.Latitude <= latitude + latDelta &&
                s.Location.Longitude >= longitude - lonDelta &&
                s.Location.Longitude <= longitude + lonDelta &&
                s.Status == SponsorStatus.Active);
        
        var candidates = await query
            .AsSplitQuery()
            .Select(s => new
            {
                Sponsor = s,
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
            .ThenByDescending(x => x.Sponsor.Status == SponsorStatus.Active)
            .Take(count)
            .Select(x => new NearbySponsorDto(
                x.Sponsor.Id,
                x.Sponsor.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Title)
                    .FirstOrDefault()!,
                x.Sponsor.Type,
                x.Sponsor.Location.Latitude,
                x.Sponsor.Location.Longitude,
                x.Sponsor.MainImageUrl,
                x.Sponsor.Offers.Any(o => o.IsActive && o.ValidityPeriod.StartDate <= now && o.ValidityPeriod.EndDate >= now),
                x.Sponsor.Offers.Count(o => o.IsActive && o.ValidityPeriod.StartDate <= now && o.ValidityPeriod.EndDate >= now)))
            .ToListAsync(cancellationToken);

        return candidates;
    }

    public async Task<SponsorOfferDto?> GetOfferByIdAsync(
        Guid offerId,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var data = await context.Offers
            .Where(o => o.Id == offerId)
            .AsNoTracking()
            .Select(o => new {
                o.Id,
                SponsorId = o.Sponsor.Id,
                SponsorName = o.Sponsor.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Title)
                    .FirstOrDefault(),
                Localized = o.LocalizedContents.Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Title, lc.Description, lc.TermsAndConditions})
                    .FirstOrDefault(),
                o.DiscountAmount,
                o.DiscountPercentage,
                o.ValidityPeriod,
                o.PromoCode,
                o.MaxRedemptions,
                o.RedemptionCount,
                o.IsActive,
                o.CreatedAt})
            .FirstOrDefaultAsync(cancellationToken);
        
        if (data == null)
            return null;

        return new SponsorOfferDto(
                data.Id,
                data.SponsorId,
                data.SponsorName!,
                data.Localized!.Title,
                data.Localized.Description,
                data.DiscountAmount != null ? new MoneyDto(data.DiscountAmount.Amount, data.DiscountAmount.Currency, data.DiscountAmount.ToString()) : null,
                data.DiscountPercentage,
                new DateRangeDto(data.ValidityPeriod.StartDate, data.ValidityPeriod.EndDate, data.ValidityPeriod.DurationInDays),
                data.ValidityPeriod.StartDate <= now && data.ValidityPeriod.EndDate >= now,
                EF.Functions.DateDiffDay(now, data.ValidityPeriod.EndDate),
                data.Localized.TermsAndConditions,
                data.PromoCode,
                data.MaxRedemptions,
                data.RedemptionCount,
                data.IsActive &&
                data.ValidityPeriod.StartDate <= now &&
                data.ValidityPeriod.EndDate >= now &&
                (!data.MaxRedemptions.HasValue || data.RedemptionCount < data.MaxRedemptions.Value),
                data.IsActive,
                data.CreatedAt);
    }

    public Task<List<ImageDto>> GetImagesAsync(
        Guid sponsorId,
        CancellationToken cancellationToken = default)
    {
        return Sponsors
            .Where(s => s.Id == sponsorId)
            .SelectMany(s => s.Images)
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new ImageDto(
                i.Id,
                i.StorageKey,
                i.ImageUrl,
                i.Caption,
                i.IsMain,
                i.DisplayOrder))
            .ToListAsync(cancellationToken);
    }

    public Task<ImageDto?> GetImageByIdAsync(
        Guid sponsorId,
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        return Sponsors
            .Where(s => s.Id == sponsorId)
            .SelectMany(s => s.Images)
            .Where(i => i.Id == imageId)
            .Select(i => new ImageDto(
                i.Id,
                i.StorageKey,
                i.ImageUrl,
                i.Caption,
                i.IsMain,
                i.DisplayOrder))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<SponsorOfferDto>> GetOffersAsync(
        Guid sponsorId,
        bool activeOnly = true,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var query = Sponsors.AsSplitQuery()
            .Where(s => s.Id == sponsorId)
            .SelectMany(s => s.Offers);

        if (activeOnly)
        {
            query = query.Where(o => o.IsActive);
        }

        var entities = await query
            .Select(o => new
            {
                Offer = o,
                SponsorLocalized = o.Sponsor.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Title })
                    .FirstOrDefault()!,
                OfferLocalized = o.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Title, lc.Description, lc.TermsAndConditions })
                    .FirstOrDefault()!
            })
            .OrderByDescending(x => x.Offer.CreatedAt)
            .Select(x => new SponsorOfferDto(
                x.Offer.Id,
                x.Offer.Sponsor.Id,
                x.SponsorLocalized.Title,
                x.OfferLocalized.Title,
                x.OfferLocalized.Description,
                x.Offer.DiscountAmount != null ? new MoneyDto(x.Offer.DiscountAmount.Amount, x.Offer.DiscountAmount.Currency, x.Offer.DiscountAmount.ToString()) : null,
                x.Offer.DiscountPercentage,
                new DateRangeDto(x.Offer.ValidityPeriod.StartDate, x.Offer.ValidityPeriod.EndDate, x.Offer.ValidityPeriod.DurationInDays),
                x.Offer.ValidityPeriod.StartDate <= now && x.Offer.ValidityPeriod.EndDate >= now,
                EF.Functions.DateDiffDay(now, x.Offer.ValidityPeriod.EndDate),
                x.OfferLocalized != null ? x.OfferLocalized.TermsAndConditions : null,
                x.Offer.PromoCode,
                x.Offer.MaxRedemptions,
                x.Offer.RedemptionCount,
                x.Offer.IsActive &&
                x.Offer.ValidityPeriod.StartDate <= now &&
                x.Offer.ValidityPeriod.EndDate >= now &&
                (!x.Offer.MaxRedemptions.HasValue || x.Offer.RedemptionCount < x.Offer.MaxRedemptions.Value),
                x.Offer.IsActive,
                x.Offer.CreatedAt))
            .ToListAsync(cancellationToken);

        return entities;
    }

    public Task<AdminOfferLocalizedContentDto?> GetOfferLocalizedContentByIdAsync(
        Guid offerId,
        Guid contentId,
        CancellationToken cancellationToken = default)
    {
        return context.Offers.AsNoTracking()
            .Where(t => t.Id == offerId)
            .SelectMany(t => t.LocalizedContents)
            .Where(lc => lc.Id == contentId)
            .Select(lc => new AdminOfferLocalizedContentDto(
                default,
                lc.Language,
                lc.Title,
                lc.Description,
                lc.TermsAndConditions,
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

    public Task<List<SponsorOfferSummaryDto>> GetActiveOffersAsync(
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return GetActiveOffersInternalAsync(count, language, now, cancellationToken);
    }

    private async Task<List<SponsorOfferSummaryDto>> GetActiveOffersInternalAsync(
        int count,
        LanguageCode language,
        DateTime now,
        CancellationToken cancellationToken)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        return await db.Sponsors.AsNoTracking()
            .SelectMany(s => s.Offers)
            .Where(o =>
                o.IsActive &&
                o.ValidityPeriod.StartDate <= now &&
                o.ValidityPeriod.EndDate >= now &&
                (!o.MaxRedemptions.HasValue || o.RedemptionCount < o.MaxRedemptions.Value))
            .Select(o => new
            {
                Offer = o,
                SponsorLocalized = o.Sponsor.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Title })
                    .FirstOrDefault()!,
                OfferLocalized = o.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Title, lc.Description })
                    .FirstOrDefault()!
            })
            .OrderByDescending(x => x.Offer.DiscountPercentage)
            .ThenByDescending(x => x.Offer.CreatedAt)
            .Take(count)
            .Select(x => new SponsorOfferSummaryDto(
                x.Offer.Id,
                x.Offer.Sponsor.Id,
                x.SponsorLocalized.Title,
                x.OfferLocalized.Title,
                x.OfferLocalized.Description,
                x.Offer.DiscountAmount != null ? new MoneyDto(x.Offer.DiscountAmount.Amount, x.Offer.DiscountAmount.Currency, x.Offer.DiscountAmount.ToString()) : null,
                x.Offer.DiscountPercentage,
                EF.Functions.DateDiffDay(now, x.Offer.ValidityPeriod.EndDate)))
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<SponsorListDto>> SearchAsync(
        string searchTerm,
        SponsorFilters filters,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var term = searchTerm.Trim();
        var query = ApplyFilters(Sponsors, filters)
            .Select(s => new
            {
                Sponsor = s,
                Localized = s.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => new { lc.Title, lc.Description })
                    .FirstOrDefault()!
            })
            .Where(x =>
                x.Localized != null &&
                EF.Functions.Like(x.Localized.Title, $"%{term}%"))
            .OrderByDescending(x => x.Sponsor.CreatedAt)
            .Select(x => new SponsorListDto(
                x.Sponsor.Id,
                x.Localized.Title,
                x.Sponsor.Type,
                x.Sponsor.Location.Latitude,
                x.Sponsor.Location.Longitude,
                x.Sponsor.MainImageUrl,
                x.Sponsor.ContractDate.StartDate <= now && x.Sponsor.ContractDate.EndDate >= now,
                x.Sponsor.Status,
                x.Sponsor.Offers.Count(o => o.IsActive),
                null));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<SponsorListDto>(items, totalCount, paging.Page, paging.PageSize);
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
            query = query.Where(s => s.Status == SponsorStatus.Active);
        }

        return query;
    }

    public async Task<List<AdminSponsorLocalizedContentDto>> GetLocalizedContentsAsync(Guid sponsorId, CancellationToken cancellationToken = default)
    {
        return await Sponsors
            .Where(s => s.Id == sponsorId)
            .SelectMany(s => s.LocalizedContents)
            .Select(lc => new AdminSponsorLocalizedContentDto(
                lc.Id,
                lc.Language,
                lc.Title,
                lc.Description,
                lc.Address,
                new(
                    lc.CreatedAt,
                    lc.CreatedBy,
                    lc.CreatedByName,
                    lc.LastModifiedAt,
                    lc.LastModifiedBy,
                    lc.LastModifiedByName)
            )).ToListAsync(cancellationToken);
    }

    public async Task<AdminSponsorLocalizedContentDto?> GetLocalizedContentByIdAsync(
        Guid sponsorId, Guid contentId, CancellationToken cancellationToken = default)
    {
        return await Sponsors
            .Where(s => s.Id == sponsorId)
            .SelectMany(s => s.LocalizedContents)
            .Where(lc => lc.Id == contentId)
            .Select(lc => new AdminSponsorLocalizedContentDto(
                lc.Id,
                lc.Language,
                lc.Title,
                lc.Description,
                lc.Address,
                new (
                    lc.CreatedAt,
                    lc.CreatedBy,
                    lc.CreatedByName,
                    lc.LastModifiedAt,
                    lc.LastModifiedBy,
                    lc.LastModifiedByName)
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<AdminOfferLocalizedContentDto>> GetOfferLocalizedContentsAsync(
        Guid offerId,
        CancellationToken cancellationToken = default)
    {
        return context.Offers.AsNoTracking()
            .Where(t => t.Id == offerId)
            .SelectMany(t => t.LocalizedContents)
            .Select(lc => new AdminOfferLocalizedContentDto(
                lc.Id,
                lc.Language,
                lc.Title,
                lc.Description,
                lc.TermsAndConditions,
                new AuditInfoDto(
                    lc.CreatedAt,
                    lc.CreatedBy,
                    lc.CreatedByName,
                    lc.LastModifiedAt,
                    lc.LastModifiedBy,
                    lc.LastModifiedByName)
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SponsorMapMarkerDto>> GetNearbyMarkerAsync(
        double latitude,
        double longitude,
        int radiusKm = 40,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var latDelta = radiusKm / 111.0;
        var lonDelta = radiusKm / (111.0 * Math.Cos(latitude * Math.PI / 180.0));

        var minLat = latitude - latDelta;
        var maxLat = latitude + latDelta;
        var minLon = longitude - lonDelta;
        var maxLon = longitude + lonDelta;

        var sponsors = Sponsors
            .Where(a => a.Status == SponsorStatus.Active)
            .Where(s => 
                s.Location.Latitude >= minLat &&
                s.Location.Latitude <= maxLat &&
                s.Location.Longitude >= minLon &&
                s.Location.Longitude <= maxLon)
            .Select(s => new
            {
                Sponsor = s,
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
        
        return await sponsors
            .OrderByDescending(s => s.Sponsor.Tier)
            .Take(count)
            .Select(s => new SponsorMapMarkerDto(
                s.Sponsor.Id,
                s.Sponsor.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Title)
                    .FirstOrDefault()!,
                new LocationDto(s.Sponsor.Location.Latitude, s.Sponsor.Location.Longitude),
                s.Sponsor.Images.Where(img => img.IsMain).Select(img => img.ImageUrl).FirstOrDefault()
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminSponsorDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var dashboardData = await Sponsors
            .GroupBy(_ => 1)
            .Select(g => new AdminSponsorDashboardDto(
                TotalSponsors: g.Count(),
                ActiveSponsors: g.Count(s => s.Status == SponsorStatus.Active),
                ExpiredSponsors: g.Count(s => s.Status == SponsorStatus.Expired),
                TotalOffers: g.Sum(s => s.Offers.Count),
                ActiveOffers: g.Sum(s => s.Offers.Count(o => o.IsActive))
            ))
            .FirstOrDefaultAsync(cancellationToken);
        
        return dashboardData ?? new AdminSponsorDashboardDto(0, 0, 0, 0, 0);
    }

    public Task<bool> AnyAsync(Guid sponsorId, CancellationToken cancellationToken)
    {
        return context.Sponsors.AnyAsync(s => s.Id == sponsorId, cancellationToken);
    }
}
