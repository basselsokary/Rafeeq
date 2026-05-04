using Domain.Common;

namespace Domain.ValueObjects;

public sealed class StorageKey : ValueObject
{
    public string Value { get; } = null!;

    private StorageKey() {}
    private StorageKey(string value) => Value = value;

    public static StorageKey General(string ext) =>
        new($"images/general/{Guid.NewGuid()}{ext}");
    
    public static StorageKey ForCitiesImages(string ext) =>
        new($"images/cities/{Guid.NewGuid()}{ext}");
    
    public static StorageKey ForSiteImages(string ext) =>
        new($"images/sites/{Guid.NewGuid()}{ext}");
    
    public static StorageKey ForAttractionImages(string ext) =>
        new($"images/attractions/{Guid.NewGuid()}{ext}");
    
    public static StorageKey ForSponsorImages(string ext) =>
        new($"images/sponsors/{Guid.NewGuid()}{ext}");

    public override string ToString() => Value;

    public static implicit operator string(StorageKey key) => key.Value;
    public static implicit operator StorageKey(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}