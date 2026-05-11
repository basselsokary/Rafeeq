using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class CachedReviewQueryService(IReviewQueryService inner, IMemoryCache cache)
    : BaseCache("review", cache), IReviewQueryService
{
    public async Task<ReviewDetailDto?> GetByIdAsync(Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:detail:{id}";
        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByIdAsync(id, language, cancellationToken));
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
        var key = $"{Prefix}:site:{siteId}:rating={rating?.ToString() ?? "all"}:order={orderBy}:status={status}:{FormatPaging(paging)}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetBySiteIdAsync(siteId, paging, rating, orderBy, status, language, cancellationToken));
    }

    public async Task<List<ReviewSummaryDto>> GetRecentBySiteIdAsync(Guid siteId, int count = 5, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:site:{siteId}:recent:{count}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetRecentBySiteIdAsync(siteId, count, cancellationToken));
    }

    public async Task<List<ReviewSummaryDto>> GetTopHelpfulBySiteIdAsync(Guid siteId, int count = 5, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:site:{siteId}:top-helpful:{count}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetTopHelpfulBySiteIdAsync(siteId, count, cancellationToken));
    }

    public async Task<PagedResult<TouristReviewDto>> GetByTouristIdAsync(Guid touristId, PagingParameters paging, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:tourist:{touristId}:{FormatPaging(paging)}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetByTouristIdAsync(touristId, paging, language, cancellationToken));
    }

    public async Task<List<TouristReviewDto>> GetRecentByTouristIdAsync(Guid touristId, int count = 5, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:tourist:{touristId}:recent:{count}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetRecentByTouristIdAsync(touristId, count, language, cancellationToken));
    }

    public async Task<PagedResult<ReviewListDto>> GetByStatusAsync(
        PagingParameters paging,
        ReviewStatus status = ReviewStatus.Pending,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:status:{status}:{FormatPaging(paging)}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetByStatusAsync(paging, status, language, cancellationToken));
    }
}