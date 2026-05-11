using Application.Common.Interfaces.Services;
using Application.DTOs.Artifacts;

namespace Application.Services;

public class ScannerService(IImageScannerService scannerService)
{
    public async Task<Result<ScanImageResponse>> ScanArtifactAsync(Stream imageStream, string contentType, CancellationToken cancellationToken = default)
    {
        return await scannerService.ScanAsync(imageStream, contentType, cancellationToken);
    }
}