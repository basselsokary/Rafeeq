using Microsoft.EntityFrameworkCore;
using Domain.Entities.ReviewAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal sealed class ReviewRepository(ApplicationDbContext context)
    : BaseRepository<Review>(context), IReviewRepository
{
    public async Task<bool> HasUserReviewedSiteAsync(Guid userId, Guid siteId, CancellationToken cancellationToken = default)
        => await DbSet
            .AnyAsync(
            r => r.TouristId == userId && r.SiteId == siteId,
            cancellationToken);

}
