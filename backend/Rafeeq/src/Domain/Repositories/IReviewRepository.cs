using Domain.Entities.ReviewAggregate;

namespace Domain.Repositories;

public interface IReviewRepository : IBaseRepository<Review>
{
    Task<bool> HasUserReviewedSiteAsync(Guid userId, Guid siteId, CancellationToken cancellationToken = default);
}
