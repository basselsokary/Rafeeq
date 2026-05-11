using Domain.Common;
using Shared;

namespace Domain.ValueObjects;

public sealed class ImageContentType : ValueObject
{
    public static readonly ImageContentType Jpeg = new("image/jpeg");
    public static readonly ImageContentType Png  = new("image/png");
    public static readonly ImageContentType WebP = new("image/webp");

    public string Value { get; } = null!;

    private ImageContentType() {}
    private ImageContentType(string value) => Value = value;

    private static readonly Dictionary<string, ImageContentType> _byExtension = new()
    {
        [".jpg"]  = Jpeg,
        [".jpeg"] = Jpeg,
        [".png"]  = Png,
        [".webp"] = WebP,
    };

    public static Result<ImageContentType> FromExtension(string ext)
    {
        if (_byExtension.TryGetValue(ext.ToLowerInvariant(), out var ct))
            return Result.Success(ct);

        return Result.Failure<ImageContentType>(
            FileErrors.UnsupportedType(ext));
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator ImageContentType(string value) => new(value);
}