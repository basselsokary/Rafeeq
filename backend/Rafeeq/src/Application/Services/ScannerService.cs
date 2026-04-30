using Application.Common.Interfaces.Services;

namespace Application.Services;

public class ScannerService(IImageScannerService scannerService)
{
    public async Task<Result<ImageResult>> ScanArtifactAsync(Stream imageStream, string contentType, CancellationToken cancellationToken = default)
    {
        return await scannerService.ScanAsync(imageStream, contentType, cancellationToken);
    }
}