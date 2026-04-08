using Domain.Entities.AttractionAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal class AttractionRepository(ApplicationDbContext context)
    : BaseRepository<Attraction>(context), IAttractionRepository
{
    public Task<Attraction?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Attractions.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<Attraction?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Attractions.Include(a => a.LocalizedContents).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
