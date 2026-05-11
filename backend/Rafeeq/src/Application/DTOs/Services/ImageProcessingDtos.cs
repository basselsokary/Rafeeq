using Domain.ValueObjects;

namespace Application.DTOs.Services;

public sealed record ProcessedImage(
    Stream Stream,
    ImageContentType ContentType,
    long SizeBytes) : IAsyncDisposable
{
    public async ValueTask DisposeAsync() => await Stream.DisposeAsync();
}