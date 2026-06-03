using Application.Common.Interfaces.QueryServices;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity.Entities;
using Domain.Entities.TouristAggregate;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class TouristQueryService(
    ApplicationDbContext context) : ITouristQueryService
{
    private IQueryable<Tourist> Tourists => context.Tourists.AsNoTracking();
    private IQueryable<TouristUser> Users => context.TouristUsers.AsNoTracking();

    public async Task<TouristProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Tourists
            .Join(
                Users,
                tourist => tourist.Id,
                user => user.Id,
                (tourist, user) => new
                {
                    tourist,
                    user
                })
            .Where(x => x.tourist.Id == id)
            .Select(x => new TouristProfileDto(
                x.tourist.Id,
                x.user.UserName!,
                x.tourist.FirstName,
                x.tourist.LastName,
                x.tourist.FirstName + " " + x.tourist.LastName,
                x.user.Email!,
                x.tourist.Nationality,
                x.tourist.TotalTrips,
                x.tourist.TotalRatings,
                x.tourist.CreatedAt,
                x.user.LastLoginAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetFavoriteSiteIdsAsync(Guid touristId, CancellationToken cancellationToken = default)
    {
        return await Tourists
            .Where(t => t.Id == touristId)
            .SelectMany(t => t.Favourites)
            .Select(f => f.SiteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<FavoriteSiteDto>> GetFavoriteSitesAsync(
        Guid touristId,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Tourists
            .Where(tourist => tourist.Id == touristId)
            .SelectMany(
                tourist => tourist.Favourites,
                (tourist, favourite) => new
                {
                    favourite
                })
            .Join(
                context.Sites.AsNoTracking(),
                x => x.favourite.SiteId,
                site => site.Id,
                (x, site) => new
                {
                    x.favourite,
                    site
                })
            .OrderByDescending(x => x.favourite.CreatedAt)
            .Select(x => new FavoriteSiteDto(
                x.favourite.Id,
                x.site.Id,
                x.site.LocalizedContents.Where(c => c.Language == language || c.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(c => c.Name)
                    .FirstOrDefault()!,
                x.site.Type,
                x.site.MainImageUrl,
                x.site.AverageRating,
                x.favourite.Notes,
                x.favourite.CreatedAt));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<FavoriteSiteDto>(
            items,
            totalCount,
            paging.Page,
            paging.PageSize);
    }

    public async Task<PagedResult<VisitedSiteDto>> GetVisitedSitesAsync(Guid id, PagingParameters paging, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var query = Tourists
            .Where(tourist => tourist.Id == id)
            .SelectMany(
                tourist => tourist.VisitedSites,
                (tourist, visited) => new
                {
                    visited
                })
            .Join(
                context.Sites.AsNoTracking(),
                x => x.visited.SiteId,
                site => site.Id,
                (x, site) => new
                {
                    x.visited,
                    site
                })
            .OrderByDescending(x => x.visited.CreatedAt)
            .Select(x => new VisitedSiteDto(
                x.visited.Id,
                x.site.Id,
                x.site.LocalizedContents.Where(c => c.Language == language || c.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(c => c.Name)
                    .FirstOrDefault()!,
                x.site.Type,
                x.site.MainImageUrl,
                x.site.AverageRating,
                x.visited.VisitDate,
                x.visited.DurationMinutes,
                x.visited.Rating ?? 0,
                x.visited.Rating != null,
                x.visited.Notes,
                x.site.Type.ToString()));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<VisitedSiteDto>(
            items,
            totalCount,
            paging.Page,
            paging.PageSize);
    }

    public async Task<bool> HasFavoritedSiteAsync(Guid touristId, Guid siteId, CancellationToken cancellationToken = default)
    {
        return await Tourists
            .Where(t => t.Id == touristId)
            .SelectMany(t => t.Favourites)
            .AnyAsync(f => f.SiteId == siteId, cancellationToken);
    }
}
