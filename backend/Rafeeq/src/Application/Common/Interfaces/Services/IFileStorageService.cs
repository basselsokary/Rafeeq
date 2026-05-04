using Application.Common.Models;
using Domain.ValueObjects;

namespace Application.Common.Interfaces.Services;

public interface IFileStorageService
{
    /// <summary>
    /// Uploads a stream to cloud storage under the given storage key.
    /// Returns the public (or SAS) URL for the stored file.
    /// </summary>
    Task<UploadedFileResult> UploadAsync(
        Stream fileStream,
        StorageKey storageKey,
        CancellationToken ct = default);

    Task<Result> DeleteAsync(StorageKey storageKey, CancellationToken ct = default);
    Task<Result> DeleteAsync(IEnumerable<StorageKey> storageKeys, CancellationToken ct = default);

    Task<bool> ExistsAsync(StorageKey storageKey, CancellationToken ct = default);
    
    Task<string> GetUrlAsync(StorageKey storageKey, CancellationToken ct = default);
}
