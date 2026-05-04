using Microsoft.EntityFrameworkCore;
using Domain.Entities.SponsorAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal sealed class SponsorRepository(ApplicationDbContext context)
    : BaseRepository<Sponsor>(context), ISponsorRepository
{
    public Task<Offer?> GetOfferByIdAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        return DbContext.Offers
            .Include(o => o.LocalizedContents)
            .FirstOrDefaultAsync(o => o.Id == offerId, cancellationToken);
    }

    public async Task<Sponsor?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.Include(s => s.Images)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<Sponsor?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet.Include(s => s.LocalizedContents)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sponsor?> GetWithOffersAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.Include(s => s.Offers)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

}
