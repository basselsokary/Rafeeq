using Microsoft.EntityFrameworkCore;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;
using Domain.Entities;
using Domain.ValueObjects;

namespace Infrastructure.Persistence.Repositories;

internal sealed class StoredFileRepository(ApplicationDbContext context) : IStoredFileRepository
{
    private readonly DbSet<StoredFile> DbSet = context.Set<StoredFile>();
    
    public async Task AddAsync(StoredFile file, CancellationToken ct = default)
    {
        await DbSet.AddAsync(file, ct);
    }

    public async Task<StoredFile?> FindByHashAsync(FileHash hash, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(f => f.Hash.Value == hash.Value, ct);
    }

    public async Task<IEnumerable<StoredFile>> FindByHashsAsync(List<FileHash> hash, CancellationToken ct = default)
    {
        var hashValues = hash.Select(h => h.Value).ToList();
        return await DbSet.Where(f => hashValues.Contains(f.Hash.Value)).ToListAsync(ct);
    }

    public async Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<IEnumerable<StoredFile>> GetByIdsAsync(List<Guid> ids, CancellationToken ct = default)
    {
        return await DbSet.Where(f => ids.Contains(f.Id)).ToListAsync(ct);
    }
}
