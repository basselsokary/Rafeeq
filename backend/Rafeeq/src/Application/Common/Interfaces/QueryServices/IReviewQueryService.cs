using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IReviewQueryService
{
    Task<ReviewDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<ReviewListDto>> GetBySiteIdAsync(
        Guid siteId,
        string? sortBy = "Helpful", // Helpful, Recent, Rating
        PagingParameters? paging = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ReviewListDto>> GetApprovedBySiteIdAsync(
        Guid siteId,
        string? sortBy = "Helpful",
        PagingParameters? paging = null,
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
        PagingParameters? paging = null,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<UserReviewDto>> GetByUserIdAsync(
        Guid userId,
        PagingParameters? paging = null,
        CancellationToken cancellationToken = default);
    
    Task<List<UserReviewDto>> GetRecentByUserIdAsync(
        Guid userId,
        int count = 5,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ReviewListDto>> GetByStatusAsync(
        ReviewStatus status = ReviewStatus.Pending,
        PagingParameters? paging = null,
        CancellationToken cancellationToken = default);
}
