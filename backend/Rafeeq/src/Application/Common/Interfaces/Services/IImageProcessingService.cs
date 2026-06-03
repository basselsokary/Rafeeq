using Application.DTOs.Services;
using Domain.ValueObjects;

namespace Application.Common.Interfaces.Services;

public interface IImageProcessingService
{
    Task<Result<ProcessedImage>> ProcessAsync(
        Stream imageStream,
        ImageContentType contentType,
        CancellationToken ct = default);

    Task<Result<ProcessedImage>> CompressForMlAsync(
        Stream imageStream,
        ImageContentType contentType,
        CancellationToken ct = default);
}
