using Application.DTOs.Services;
using Domain.ValueObjects;

namespace Application.Common.Interfaces.Services;

public interface IImageProcessingService
{
    /// <summary>
    /// Resizes/compresses the image if it exceeds configured max dimensions.
    /// Returns a new stream containing the processed image, and the
    /// (possibly updated) content type.
    /// Original stream is NOT disposed — caller owns it.
    /// </summary>
    Task<Result<ProcessedImage>> ProcessAsync(
        Stream imageStream,
        ImageContentType contentType,
        CancellationToken ct = default);
}
