using Domain.Common;

namespace Domain.ValueObjects;

public sealed class FileHash : ValueObject
{
    public string Value { get; } = null!; // Lowercase hex SHA-256

    private FileHash() { }
    private FileHash(string value) => Value = value;
    
    public static FileHash From(string hexHash)
    {
        return new(hexHash.ToLowerInvariant());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(FileHash hash) => hash.Value;
}
