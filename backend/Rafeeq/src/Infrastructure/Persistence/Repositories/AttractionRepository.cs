using Domain.Entities.AttractionAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class AttractionRepository(ApplicationDbContext context)
    : BaseRepository<Attraction>(context), IAttractionRepository
{
    public async Task<Attraction?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.Include(a => a.Images)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<Attraction?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.Include(a => a.LocalizedContents)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
