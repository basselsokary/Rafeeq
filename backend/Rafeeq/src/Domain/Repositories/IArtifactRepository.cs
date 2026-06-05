using Domain.Entities.ArtifactAggregate;

namespace Domain.Repositories;

public interface IArtifactRepository : IBaseRepository<Artifact>
{
	Task<Artifact?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Artifact?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default);
}
