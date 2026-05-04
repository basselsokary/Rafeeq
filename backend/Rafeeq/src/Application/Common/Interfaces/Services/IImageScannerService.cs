namespace Application.Common.Interfaces.Services;

public interface IImageScannerService
{
    Task<Result<ImageResult>> ScanAsync(
        Stream imageStream,
        string contentType,
        CancellationToken ct = default);
}

public record ImageResult(
    string Name,
    string Description,
    string SiteName,
    List<string> Images);
