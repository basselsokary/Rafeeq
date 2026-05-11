using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface IStoredFileRepository
{
    /// <summary>
    /// Looks across ALL uploaded images regardless of which domain
    /// entity (site, sponsor, attraction, etc.) they were uploaded for.
    /// </summary>
    Task<IEnumerable<StoredFile>> GetByIdsAsync(List<Guid> ids, CancellationToken ct = default);
    Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IEnumerable<StoredFile>> FindByHashsAsync(List<FileHash> hash, CancellationToken ct = default);
    Task<StoredFile?> FindByHashAsync(FileHash hash, CancellationToken ct = default);

    Task AddAsync(StoredFile file, CancellationToken ct = default);
}
