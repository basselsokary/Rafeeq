using Application.DTOs.Integrations.Scanner;

namespace Application.Common.Interfaces.Services;

public interface IImageScannerService
{
    Task<Result<ScanImageResponse>> ScanAsync(
        Stream imageStream, string contentType, CancellationToken ct = default);
    
    Task<Result<ScanImageResponseV2>> ScanAsyncV2(
        Stream imageStream, string contentType, CancellationToken ct = default);
}