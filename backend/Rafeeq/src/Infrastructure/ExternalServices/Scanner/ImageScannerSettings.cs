namespace Infrastructure.ExternalServices.Scanner;

public sealed class ImageScannerSettings
{
    public const string SectionName = "ImageScannerService";

    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>
    /// Endpoint path on the analysis service that accepts image uploads.
    /// e.g. "/predict" or "/api/v1/predict"
    /// </summary>
    public string ScanEndpoint { get; init; } = "/predict";

    /// <summary>
    /// The multipart form-data field name the service expects.
    /// Match exactly what the service reads on the other end.
    /// </summary>
    public string ImageFieldName { get; init; } = "file";

    public int TimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// Optional static API key sent as a header.
    /// Leave empty for local development — set it in production secrets.
    /// </summary>
    public string ApiKey { get; init; } = string.Empty;

    public string ApiKeyHeaderName { get; init; } = "X-Api-Key";
}