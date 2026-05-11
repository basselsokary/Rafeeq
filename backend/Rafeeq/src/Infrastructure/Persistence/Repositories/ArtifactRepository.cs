using Domain.Entities.ArtifactAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal sealed class ArtifactRepository(ApplicationDbContext context)
    : BaseRepository<Artifact>(context), IArtifactRepository
{
}
