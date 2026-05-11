using Application.DTOs.Artifacts;

namespace Application.Common.Interfaces.Services;

public interface IImageScannerService
{
    Task<Result<ScanImageResponse>> ScanAsync(
        Stream imageStream, string contentType, CancellationToken ct = default);
}