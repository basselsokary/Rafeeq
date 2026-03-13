using Microsoft.EntityFrameworkCore;
using Domain.Entities.ReviewAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal class ReviewRepository(ApplicationDbContext context)
    : BaseRepository<Review>(context), IReviewRepository
{
    public Task<bool> HasUserReviewedSiteAsync(Guid userId, Guid siteId, CancellationToken cancellationToken = default)
        => _dbSet
            .AnyAsync(
            r => r.UserId == userId && r.SiteId == siteId,
            cancellationToken);

}
