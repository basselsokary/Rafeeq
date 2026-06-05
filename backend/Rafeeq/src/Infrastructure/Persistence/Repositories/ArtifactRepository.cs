using Domain.Entities.ArtifactAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class ArtifactRepository(ApplicationDbContext context)
    : BaseRepository<Artifact>(context), IArtifactRepository
{
    public Task<Artifact?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet.Include(a => a.Images)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<Artifact?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet.Include(a => a.LocalizedContents)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
