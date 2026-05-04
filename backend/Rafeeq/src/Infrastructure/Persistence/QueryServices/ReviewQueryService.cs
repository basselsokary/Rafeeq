using System.Linq.Expressions;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Domain.Entities.ReviewAggregate;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class ReviewQueryService(
    ApplicationDbContext context) : IReviewQueryService
{
    private IQueryable<Review> Reviews => context.Reviews.AsNoTracking();

    public async Task<ReviewDetailDto?> GetByIdAsync(Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        return await Reviews
            .Where(r => r.Id == id)
            .AsSplitQuery()
            .Select(r => new ReviewDetailDto(
                r.Id,
                r.SiteId,
                r.Site.LocalizedContents.Where(c => c.Language == language)
                    .Select(c => c.Name)
                    .FirstOrDefault()!,
                r.Site.Type,
                r.TouristId,
                r.Tourist.FirstName,
                r.Rating,
                r.Title,
                r.Content,
                r.Status,
                r.HelpfulCount,
                r.NotHelpfulCount,
                r.HelpfulCount + r.NotHelpfulCount == 0 ? 0 : (double)r.HelpfulCount / (r.HelpfulCount + r.NotHelpfulCount),
                r.RejectionReason,
                r.CreatedAt,
                r.LastModifiedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<ReviewListDto>> GetBySiteIdAsync(
        Guid siteId,
        PagingParameters paging,
        int? rating = null,
        ReviewOrderBy orderBy = ReviewOrderBy.Helpful,
        ReviewStatus status = ReviewStatus.Approved,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Reviews
            .AsSplitQuery()
            .Where(r => r.SiteId == siteId && r.Status == status);

        if (rating.HasValue)
            query = query.Where(r => r.Rating == rating);
        
        query = ApplySort(query, orderBy);
        
        return await ToPagedResultAsync(
            query,
            paging,
            c => new ReviewListDto(
                c.Id,
                c.SiteId,
                c.Site.LocalizedContents.Where(c => c.Language == language)
                    .Select(c => c.Name)
                    .FirstOrDefault()!,
                c.TouristId,
                c.Tourist.FirstName,
                c.Rating,
                c.Title,
                c.Content,
                c.Status,
                c.HelpfulCount,
                c.NotHelpfulCount,
                c.HelpfulCount + c.NotHelpfulCount == 0 ? 0 : (double)c.HelpfulCount / (c.HelpfulCount + c.NotHelpfulCount),
                c.CreatedAt),
            cancellationToken);
    }

    public async Task<PagedResult<ReviewListDto>> GetByStatusAsync(
        PagingParameters paging,
        ReviewStatus status = ReviewStatus.Pending,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Reviews
            .AsSplitQuery()
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt);
        
        return await ToPagedResultAsync(
            query,
            paging,
            r => new ReviewListDto(
                r.Id,
                r.SiteId,
                r.Site.LocalizedContents.Where(c => c.Language == language)
                    .Select(c => c.Name)
                    .FirstOrDefault()!,
                r.TouristId,
                r.Tourist.FirstName,
                r.Rating,
                r.Title,
                r.Content,
                r.Status,
                r.HelpfulCount,
                r.NotHelpfulCount,
                r.HelpfulCount + r.NotHelpfulCount == 0 ? 0 : (double)r.HelpfulCount / (r.HelpfulCount + r.NotHelpfulCount),
                r.CreatedAt
            ),
            cancellationToken);
    }

    public async Task<PagedResult<TouristReviewDto>> GetByTouristIdAsync(
        Guid touristId,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var query = Reviews
            .Where(r => r.TouristId == touristId)
            .OrderByDescending(r => r.CreatedAt);

        return await ToPagedResultAsync(
            query,
            paging,
            r => new TouristReviewDto(
                r.Id,
                r.SiteId,
                r.Site.LocalizedContents.Where(c => c.Language == language)
                    .Select(c => c.Name)
                    .FirstOrDefault()!,
                r.Site.MainImageUrl,
                r.Site.Type,
                r.Rating,
                r.Title,
                r.Status,
                r.HelpfulCount,
                r.CreatedAt,
                r.LastModifiedAt),
            cancellationToken);
    }

    public async Task<List<ReviewSummaryDto>> GetRecentBySiteIdAsync(
        Guid siteId,
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        var items = await Reviews
            .Where(r => r.SiteId == siteId && r.Status == ReviewStatus.Approved)
            .OrderByDescending(r => r.CreatedAt)
            .Take(count)
            .Select(r => new ReviewSummaryDto(
                r.Id,
                r.Tourist.FirstName,
                r.Rating,
                r.Title,
                r.Content,
                r.HelpfulCount))
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<List<TouristReviewDto>> GetRecentByTouristIdAsync(
        Guid touristId,
        int count = 5,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var items = await Reviews
            .Where(r => r.TouristId == touristId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(count)
            .Select(r => new TouristReviewDto(
                r.Id,
                r.SiteId,
                r.Site.LocalizedContents.Where(c => c.Language == language)
                    .Select(c => c.Name)
                    .FirstOrDefault()!,
                r.Site.MainImageUrl,
                r.Site.Type,
                r.Rating,
                r.Title,
                r.Status,
                r.HelpfulCount,
                r.CreatedAt,
                r.LastModifiedAt))
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<List<ReviewSummaryDto>> GetTopHelpfulBySiteIdAsync(Guid siteId, int count = 5, CancellationToken cancellationToken = default)
    {
        var query = Reviews
            .Where(r => r.SiteId == siteId && r.Status == ReviewStatus.Approved);

        query = ApplySort(query, ReviewOrderBy.Helpful);

        var items = await query
            .Take(count)
            .Select(r => new ReviewSummaryDto(
                r.Id,
                r.Tourist.FirstName,
                r.Rating,
                r.Title,
                r.Content,
                r.HelpfulCount))
            .ToListAsync(cancellationToken);
            
        return items;
    }

    private static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        IQueryable<Review> query,
        PagingParameters paging,
        Expression<Func<Review, T>> selector,
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

    private static IQueryable<Review> ApplySort(IQueryable<Review> query, ReviewOrderBy orderBy = ReviewOrderBy.Helpful)
    {
        return (orderBy) switch
        {
            ReviewOrderBy.Helpful => query.OrderByDescending(r => r.HelpfulCount).ThenByDescending(r => r.CreatedAt),
            ReviewOrderBy.Recent => query.OrderByDescending(r => r.CreatedAt),
            ReviewOrderBy.Rating => query.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt)  
        };
    }
}