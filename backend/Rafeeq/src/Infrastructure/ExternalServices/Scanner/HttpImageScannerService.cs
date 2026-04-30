using System.Net.Http.Headers;
using System.Text.Json;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Microsoft.Extensions.Logging;
using Shared;

namespace Infrastructure.ExternalServices.Scanner;

internal sealed class HttpImageScannerService(
    HttpClient http,
    ImageScannerSettings settings,
    ILogger<HttpImageScannerService> logger) : IImageScannerService
{

    public async Task<Result<ImageResult>> ScanAsync(
        Stream imageStream,
        string contentType,
        CancellationToken ct = default)
    {
        try
        {
            logger.LogDebug(
                "Sending image to analysis service at {BaseUrl}{Endpoint}",
                settings.BaseUrl, settings.ScanEndpoint);

            // Build multipart/form-data content.
            imageStream.Position = 0;
            using var form = new MultipartFormDataContent();

            var streamContent = new StreamContent(imageStream);
            streamContent.Headers.ContentType =
                MediaTypeHeaderValue.Parse(contentType);

            form.Add(streamContent, settings.ImageFieldName, "image");

            // Reset stream
            imageStream.Position = 0;
            HttpResponseMessage response = 
                await http.PostAsync(settings.ScanEndpoint, form, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                logger.LogWarning(
                    "Scanner service returned {Status}. Body: {Body}",
                    (int)response.StatusCode, body);

                return ScannerServiceErrors.ScanFailed;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var result = DeserializeResponse(json);

            logger.LogInformation(
                "Scanner complete. Confidence: {Confidence  }",
                result.Value.Description);

            return result.Value;
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            // Timeout (not user cancellation)
            logger.LogWarning(
                "Scanner service timed out after {Timeout}s", settings.TimeoutSeconds);
            return ScannerServiceErrors.TaskCancelled;
        }
        catch (HttpRequestException ex)
        {
            // Network-level failure (service unreachable, DNS, etc.)
            logger.LogError(ex, "Scanner service unreachable at {BaseUrl}",
                settings.BaseUrl);
            return ScannerServiceErrors.ScanFailed;
        }
    }

    private static Result<ImageResult> DeserializeResponse(string json)
    {
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var confidence = root.TryGetProperty("confidence", out var confEl)
            ? confEl.GetSingle()
            : 0f;

        var label = root.TryGetProperty("label", out var descEl)
            ? descEl.GetString()
            : null;

        return new ImageResult(
            Name: label ?? "Unknown",
            Description: $"Confidence: {confidence}",
            SiteName: $"Site: {label ?? "Unknown"}",
            Images: new List<string>()
            {
                // here fake images
                "https://example.com/image1.jpg",
                "https://example.com/image2.jpg",
                "https://example.com/image3.jpg"
            }
        );
    }
}

public static class ScannerServiceErrors
{
    public static Error ScanFailed
        => Error.Failure("SCANNER_SERVICE_FAILED", "Failed to scan the image.");

    public static Error TaskCancelled
        => Error.Failure("SCANNER_TASK_CANCELLED", "Image scanning task was cancelled (timeout).");
}

