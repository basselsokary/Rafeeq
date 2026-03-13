using Domain.Entities.ReviewAggregate;
using Domain.Enums;

namespace Domain.Repositories;

public interface IReviewRepository : IBaseRepository<Review>
{
    Task<IEnumerable<Review>> GetByPlaceIdAsync(Guid placeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByStatusAsync(ReviewStatus status, CancellationToken cancellationToken = default);
    // Task<IEnumerable<Review>> GetPendingReviewsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasUserReviewedSiteAsync(Guid userId, Guid siteId, CancellationToken cancellationToken = default);
    // Task<double> GetAverageRatingForPlaceAsync(Guid placeId, CancellationToken cancellationToken = default);
}
