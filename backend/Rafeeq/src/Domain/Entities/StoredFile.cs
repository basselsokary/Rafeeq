using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Tracks every unique image ever uploaded, regardless of which domain
/// entity it belongs to. One row per unique SHA-256 hash.
/// This is the dedup registry — not a replacement for Entities Images.
/// </summary>
public sealed class StoredFile : BaseEntity
{
    public FileHash Hash { get; private set; } = null!;
    public StorageKey StorageKey { get; private set; } = null!;
    public ImageContentType ContentType { get; private set; } = null!;
    public long Size { get; private set; }
    public int ReferenceCount { get; private set; }
    public DateTimeOffset FirstUploadedAt { get; private set; }

    private StoredFile() { }
    private StoredFile(
        Guid id,
        FileHash hash,
        StorageKey storageKey,
        ImageContentType contentType,
        long size,
        DateTimeOffset firstUploadedAt) : base(id)
    {
        Hash = hash;
        StorageKey = storageKey;
        ContentType = contentType;
        Size = size;
        FirstUploadedAt = firstUploadedAt;

        ReferenceCount = 1;
    }

    public static StoredFile Create(
        Guid id,
        FileHash hash,
        StorageKey storageKey,
        ImageContentType contentType,
        long size)
    {
        return new(
            id,
            hash,
            storageKey,
            contentType,
            size,
            DateTimeOffset.UtcNow);
    }

    public void IncrementReference()
    {
        ReferenceCount++;
    }

    public void DecrementReference()
    {
        if (ReferenceCount > 0)
            ReferenceCount--;
    }
}