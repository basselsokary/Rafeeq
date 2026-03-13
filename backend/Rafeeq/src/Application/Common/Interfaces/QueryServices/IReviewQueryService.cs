using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IReviewQueryService
{
    Task<ReviewDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<ReviewListDto>> GetBySiteIdAsync(
        Guid siteId,
        PagingParameters paging,
        ReviewOrderBy orderBy = ReviewOrderBy.Helpful,
        ReviewStatus status = ReviewStatus.Approved,
        CancellationToken cancellationToken = default);
    
    Task<List<ReviewSummaryDto>> GetRecentBySiteIdAsync(
        Guid siteId,
        int count = 5,
        CancellationToken cancellationToken = default);
    
    Task<List<ReviewSummaryDto>> GetTopHelpfulBySiteIdAsync(
        Guid siteId,
        int count = 5,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<ReviewListDto>> GetBySiteAndRatingAsync(
        Guid siteId,
        int rating,
        PagingParameters paging,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<UserReviewDto>> GetByUserIdAsync(
        Guid userId,
        PagingParameters paging,
        CancellationToken cancellationToken = default);
    
    Task<List<UserReviewDto>> GetRecentByUserIdAsync(
        Guid userId,
        int count = 5,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ReviewListDto>> GetByStatusAsync(
        PagingParameters paging,
        ReviewStatus status = ReviewStatus.Pending,
        CancellationToken cancellationToken = default);
}

public enum ReviewOrderBy
{
    Helpful,
    Recent,
    Rating
}