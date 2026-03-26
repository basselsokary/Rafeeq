using Application.Common.Interfaces.QueryServices;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;

namespace Infrastructure.Persistence.QueryServices;

internal class TouristQueryService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager) : ITouristQueryService
{
    public async Task<PagedResult<TouristListDto>> GetAllAsync(
        PagingParameters paging,
        string? searchTerm = null,
        UserStatus status = UserStatus.Active,
        bool? emailVerified = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Tourists
            .AsNoTracking()
            .Where(tourist => tourist.Status == status)
            .Join(
                context.Users.AsNoTracking(),
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
                (x.user.Email != null && EF.Functions.Like(x.user.Email, $"%{term}%")));
        }

        var queryPaging = query
            .OrderByDescending(x => x.tourist.CreatedAt)
            .Select(x => new TouristListDto(
                x.tourist.Id,
                x.tourist.FirstName + " " + x.tourist.LastName,
                x.user.Email ?? string.Empty,
                UserRole.Tourist.ToString(),
                x.tourist.TotalTrips,
                x.tourist.TotalReviews));

        var totalCount = await queryPaging.CountAsync(cancellationToken);
        var items = await queryPaging
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<TouristListDto>(
            items,
            totalCount,
            paging.PageNumber,
            paging.PageSize);
    }

    public async Task<TouristProfileDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = userManager.NormalizeEmail(email.Trim());

        return await context.Tourists
            .AsNoTracking()
            .Join(
                context.Users.AsNoTracking(),
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
                x.tourist.FirstName,
                x.tourist.LastName,
                x.tourist.FirstName + " " + x.tourist.LastName,
                x.user.Email ?? string.Empty,
                x.tourist.PreferredLanguage.ToString(),
                x.tourist.Nationality,
                x.tourist.TotalTrips,
                x.tourist.TotalReviews,
                x.tourist.CreatedAt,
                x.user.LastLoginAt ?? x.user.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TouristAdminDetailDto?> GetByEmailForAdminAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = userManager.NormalizeEmail(email.Trim());

        return await context.Tourists
            .AsNoTracking()
            .Join(
                context.Users.AsNoTracking(),
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
                x.user.Email ?? string.Empty,
                x.tourist.Status.ToString(),
                x.tourist.PreferredLanguage.ToString(),
                x.tourist.Nationality,
                x.user.EmailConfirmed,
                x.user.EmailConfirmed ? x.user.CreatedAt : null,
                // x.tourist.TotalTrips,
                // x.tourist.TotalTrips,
                x.tourist.TotalReviews,
                x.TotalFavorites,
                x.tourist.Status == UserStatus.Banned,
                x.tourist.CreatedAt,
                x.tourist.LastModifiedAt,
                x.user.LastLoginAt ?? x.user.CreatedAt,
                x.user.AccessFailedCount))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TouristProfileDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Tourists
            .AsNoTracking()
            .Join(
                context.Users.AsNoTracking(),
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
                x.tourist.FirstName,
                x.tourist.LastName,
                x.tourist.FirstName + " " + x.tourist.LastName,
                x.user.Email ?? string.Empty,
                x.tourist.PreferredLanguage.ToString(),
                x.tourist.Nationality,
                x.tourist.TotalTrips,
                x.tourist.TotalReviews,
                x.tourist.CreatedAt,
                x.user.LastLoginAt ?? x.user.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TouristAdminDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await context.Tourists
            .AsNoTracking()
            .Join(
                context.Users.AsNoTracking(),
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
            model.user.Email ?? string.Empty,
            model.tourist.Status.ToString(),
            model.tourist.PreferredLanguage.ToString(),
            model.tourist.Nationality,
            model.user.EmailConfirmed,
            model.user.EmailConfirmed ? model.user.CreatedAt : null,
            // model.tourist.TotalTrips,
            // model.tourist.TotalTrips,
            model.tourist.TotalReviews,
            model.TotalFavorites,
            model.tourist.Status == UserStatus.Banned,
            model.tourist.CreatedAt,
            model.tourist.LastModifiedAt,
            model.user.LastLoginAt ?? model.user.CreatedAt,
            model.user.AccessFailedCount);
    }

    public async Task<List<Guid>> GetFavoriteSiteIdsAsync(Guid touristId, CancellationToken cancellationToken = default)
    {
        return await context.Tourists
            .AsNoTracking()
            .Where(t => t.Id == touristId)
            .SelectMany(t => t.Favourites)
            .Select(f => f.SiteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<FavoriteSiteDto>> GetFavoriteSitesAsync(Guid touristId, PagingParameters paging , CancellationToken cancellationToken = default)
    {
        var query = context.Tourists
            .AsNoTracking()
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
                x.site.Name,
                x.site.Type.ToString(),
                x.site.MainImageUrl,
                x.site.City.Name,
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
            paging.PageNumber,
            paging.PageSize);
    }

    public async Task<bool> HasFavoritedSiteAsync(Guid touristId, Guid siteId, CancellationToken cancellationToken = default)
    {
        return await context.Tourists
            .AsNoTracking()
            .Where(t => t.Id == touristId)
            .SelectMany(t => t.Favourites)
            .AnyAsync(f => f.SiteId == siteId, cancellationToken);
    }

}
