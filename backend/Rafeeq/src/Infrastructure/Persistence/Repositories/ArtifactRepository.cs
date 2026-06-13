using Domain.Entities.ArtifactAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class ArtifactRepository(ApplicationDbContext context)
    : BaseRepository<Artifact>(context), IArtifactRepository
{
    public async Task<Artifact?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.Include(a => a.Images)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<Artifact?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.Include(a => a.LocalizedContents)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
