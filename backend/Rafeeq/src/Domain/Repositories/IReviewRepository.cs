using Domain.Entities.ReviewAggregate;
using Domain.Enums;

namespace Domain.Repositories;

public interface IReviewRepository : IBaseRepository<Review>
{
    Task<bool> HasUserReviewedSiteAsync(Guid userId, Guid siteId, CancellationToken cancellationToken = default);
    // Task<double> GetAverageRatingForPlaceAsync(Guid placeId, CancellationToken cancellationToken = default);
}
