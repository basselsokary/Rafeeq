using Domain.Common;

namespace Domain.ValueObjects;

public sealed class StorageKey : ValueObject
{
    public string Value { get; } = null!;

    private StorageKey() {}
    private StorageKey(string value) => Value = value;
    
    public static StorageKey ForEntites(FileHash hash, string ext) =>
        new($"images/dedup/{hash.Value[..2]}/{hash.Value}{ext}");

    /// <summary>
    /// User-scoped key: users/{userId}/{yyyy}/{MM}/{fileId}{ext}
    /// Date prefix enables lifecycle policies and natural partitioning.
    /// </summary>
    public static StorageKey ForUserUpload(
        Guid userId, Guid fileId, string ext, DateTimeOffset at) =>
        new($"users/{userId}/{at:yyyy}/{fileId}{ext}");

    public override string ToString() => Value;

    public static implicit operator string(StorageKey key) => key.Value;
    public static implicit operator StorageKey(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}