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
        int? rating = null,
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
    
    Task<PagedResult<TouristReviewDto>> GetByTouristIdAsync(
        Guid touristId,
        PagingParameters paging,
        CancellationToken cancellationToken = default);
    
    Task<List<TouristReviewDto>> GetRecentByTouristIdAsync(
        Guid touristId,
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