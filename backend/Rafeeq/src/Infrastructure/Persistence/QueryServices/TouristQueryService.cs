using Application.Common.Interfaces.QueryServices;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity.Entities;
using Domain.Entities.TouristAggregate;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class TouristQueryService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager) : ITouristQueryService
{
    private IQueryable<Tourist> Tourists => context.Tourists.AsNoTracking();
    private IQueryable<TouristUser> Users => context.TouristUsers.AsNoTracking();

    public async Task<PagedResult<TouristListDto>> GetAllAsync(
        PagingParameters paging,
        string? searchTerm = null,
        UserStatus status = UserStatus.Active,
        bool? emailVerified = null,
        CancellationToken cancellationToken = default)
    {
        var query = Tourists
            .Where(tourist => tourist.Status == status)
            .Join(
                Users,
                tourist => tourist.Id,
                user => user.Id,
                (tourist, user) => new
                {
                    tourist,
                    user
                });

        if (emailVerified.HasValue)
        {
            query = query.Where(x => x.user.EmailConfirmed == emailVerified.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(x =>
                EF.Functions.Like(x.tourist.FirstName, $"%{term}%") ||
                EF.Functions.Like(x.tourist.LastName, $"%{term}%") ||
                EF.Functions.Like(x.user.Email, $"%{term}%"));
        }

        var queryPaging = query
            .OrderByDescending(x => x.tourist.CreatedAt)
            .Select(x => new TouristListDto(
                x.tourist.Id,
                x.user.UserName!,
                x.tourist.FirstName + " " + x.tourist.LastName,
                x.user.Email!,
                x.tourist.TotalTrips,
                x.tourist.TotalRatings));

        var totalCount = await queryPaging.CountAsync(cancellationToken);
        var items = await queryPaging
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<TouristListDto>(
            items,
            totalCount,
            paging.Page,
            paging.PageSize);
    }

    public async Task<TouristProfileDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = userManager.NormalizeEmail(email.Trim());

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
            .Where(x => x.user.NormalizedEmail == normalizedEmail)
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

    public async Task<TouristAdminDetailDto?> GetByEmailForAdminAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = userManager.NormalizeEmail(email.Trim());

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
            .Where(x => x.user.NormalizedEmail == normalizedEmail)
            .Select(x => new
            {
                x.tourist,
                x.user,
                TotalFavorites = x.tourist.Favourites.Count
            })
            .Select( x => new TouristAdminDetailDto(
                x.tourist.Id,
                x.tourist.FirstName,
                x.tourist.LastName,
                x.tourist.FirstName + " " + x.tourist.LastName,
                x.user.Email!,
                x.tourist.Status,
                x.tourist.Nationality,
                x.user.EmailConfirmed,
                x.tourist.TotalRatings,
                x.TotalFavorites,
                x.tourist.Status == UserStatus.Banned,
                x.tourist.CreatedAt,
                x.tourist.LastModifiedAt,
                x.user.LastLoginAt,
                x.user.AccessFailedCount))
            .FirstOrDefaultAsync(cancellationToken);
    }

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

    public async Task<TouristAdminDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await Tourists
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
            .Select(x => new
            {
                x.tourist,
                x.user,
                TotalFavorites = x.tourist.Favourites.Count
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (model is null)
        {
            return null;
        }

        return new TouristAdminDetailDto(
            model.tourist.Id,
            model.tourist.FirstName,
            model.tourist.LastName,
            model.tourist.FirstName + " " + model.tourist.LastName,
            model.user.Email!,
            model.tourist.Status,
            model.tourist.Nationality,
            model.user.EmailConfirmed,
            model.tourist.TotalRatings,
            model.TotalFavorites,
            model.tourist.Status == UserStatus.Banned,
            model.tourist.CreatedAt,
            model.tourist.LastModifiedAt,
            model.user.LastLoginAt,
            model.user.AccessFailedCount);
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
