using Microsoft.EntityFrameworkCore;
using Domain.Entities.SponsorAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal class SponsorRepository(ApplicationDbContext context)
    : BaseRepository<Sponsor>(context), ISponsorRepository
{
    public async Task<Sponsor?> GetWithImages(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(s => s.Images)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Sponsor?> GetWithOffers(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(s => s.Offers)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

}
