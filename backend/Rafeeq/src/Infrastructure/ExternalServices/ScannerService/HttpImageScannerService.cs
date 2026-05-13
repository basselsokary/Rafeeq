using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Interfaces.Services;
using Application.DTOs.Artifacts;
using Microsoft.Extensions.Logging;
using Shared;

namespace Infrastructure.ExternalServices.ScannerService;

internal sealed class HttpImageScannerService(
    HttpClient http,
    ImageScannerSettings settings,
    ILogger<HttpImageScannerService> logger) : IImageScannerService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<Result<ScanImageResponse>> ScanAsync(
        Stream imageStream,
        string contentType,
        CancellationToken ct = default)
    {
        try
        {
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

            var imageResponse = await response.Content.ReadFromJsonAsync<ScanImageResponse>(JsonOptions, ct);
            if (imageResponse is null)
            {
                logger.LogError("Scanner service returned null response.");
                return Result.Failure<ScanImageResponse>(ScannerServiceErrors.ScanFailed);
            }

            return Result.Success(imageResponse);
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

    public async Task<Result<ScanImageResponseV2>> ScanV2Async(
        Stream imageStream,
        string contentType,
        CancellationToken ct = default)
    {
        try
        {
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

            var imageResponse = await response.Content.ReadFromJsonAsync<ScanImageResponseV2>(JsonOptions, ct);
            if (imageResponse is null)
            {
                logger.LogError("Scanner service returned null response.");
                return Result.Failure<ScanImageResponseV2>(ScannerServiceErrors.ScanFailed);
            }

            return Result.Success(imageResponse);
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
}

public static class ScannerServiceErrors
{
    public static Error ScanFailed
        => Error.Failure("SCANNER_SERVICE_FAILED", "Failed to scan the image.");

    public static Error TaskCancelled
        => Error.Failure("SCANNER_TASK_CANCELLED", "Image scanning task was cancelled (timeout).");
}

