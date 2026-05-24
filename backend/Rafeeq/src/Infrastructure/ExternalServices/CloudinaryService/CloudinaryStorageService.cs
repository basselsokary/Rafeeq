using System.Text.Json;
using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;

namespace Infrastructure.ExternalServices.CloudinaryService;

/// <summary>
/// Implements IFileStorageService using Cloudinary as the storage backend.
///
/// Key Cloudinary-specific decisions documented inline:
///  - PublicId  = our StorageKey (slashes become folder hierarchy in Cloudinary)
///  - ResourceType.Image enforced — rejects non-image bytes server-side too
///  - Invalidate = true on delete — purges Cloudinary's CDN cache immediately
///  - Signed URLs generated locally using ApiSecret — no extra API call needed
/// </summary>
internal sealed class CloudinaryStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinaryOptions _options;
    private readonly ILogger<CloudinaryStorageService> _logger;

    public CloudinaryStorageService(
        IOptions<CloudinaryOptions> options,
        ILogger<CloudinaryStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;

        var account = new Account(_options.CloudName, _options.ApiKey, _options.ApiSecret);
        _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
    }

    public async Task<Result<UploadedFileResponse>> UploadAsync(
        Stream stream,
        StorageKey storageKey,
        string hash,
        CancellationToken ct = default)
    {
        try
        {
            var publicId = BuildPublicId(storageKey);

            var uploadParams = new ImageUploadParams
            {
                PublicId = publicId,
                File = new FileDescription(storageKey, stream),
                Overwrite = false,
                Context = new StringDictionary
                {
                    { "sha256", hash },
                },
            };

            var result = await _cloudinary.UploadAsync(uploadParams, ct);

            if (result.Error != null)
            {
                _logger.LogError("Upload failed: {Message}", result.Error.Message);
                return Result.Failure<UploadedFileResponse>(CloudinaryStorageErrors.UploadFailed);
            }

            _logger.LogInformation("Upload succeeded: {Key} -> {Url}", storageKey, result.SecureUrl.ToString());

            return Result.Success(new UploadedFileResponse(storageKey, result.SecureUrl.ToString(), result.Bytes));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed for {Key}", storageKey);
            return Result.Failure<UploadedFileResponse>(CloudinaryStorageErrors.UploadFailed);
        }
    }

    public async Task<Result> DeleteAsync(StorageKey storageKey, CancellationToken ct = default)
    {
        try
        {
            var publicId = BuildPublicId(storageKey);

            var result = await _cloudinary.DestroyAsync(
                new DeletionParams(publicId));

            if (result.Result is not ("ok" or "not found"))
                return Result.Failure(CloudinaryStorageErrors.DeleteFailed);
            
            _logger.LogInformation("Delete succeeded: {Key}", storageKey);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete failed for {Key}", storageKey);
            return Result.Failure(CloudinaryStorageErrors.DeleteFailed);
        }
    }

    public async Task<Result> DeleteAsync(IEnumerable<StorageKey> storageKeys, CancellationToken ct = default)
    {
        if (!storageKeys.Any())
        {
            _logger.LogWarning("Batch delete called with empty key list");
            return Result.Success();
        }
        
        try
        {
            // var publicIds = storageKeys.Select(BuildPublicId).ToArray();
            // var result = await _cloudinary.DeleteResourcesAsync(ResourceType.Image, publicIds);
            var publicIds = storageKeys.Select(BuildPublicId).ToList();

            var deleteParams = new DelResParams
            {
                PublicIds = publicIds,
                ResourceType = ResourceType.Image,
                Invalidate = true
            };

            var result = await _cloudinary.DeleteResourcesAsync(deleteParams);
            Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));

            if (result.Deleted.All(kv => kv.Value is "deleted" or "ok"))
            {
                _logger.LogInformation("Batch delete succeeded: {Keys}", string.Join(", ", storageKeys));
                return Result.Success();
            }
            else if (result.Deleted.All(kv => kv.Value is "deleted" or "ok" or "not_found"))
            {
                _logger.LogInformation("Batch delete maybe succeeded with some not found: {Keys}", string.Join(", ", storageKeys));
                return Result.Success();
            }
            else
            {
                var failedKeys = result.Deleted.Where(kv => kv.Value != "ok" && kv.Value != "not_found")
                    .Select(kv => kv.Key);
                
                _logger.LogError("Batch delete failed for keys: {Keys}", string.Join(", ", failedKeys));
                
                return Result.Failure(CloudinaryStorageErrors.BatchDeleteFailedForKeys(failedKeys));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Batch delete failed for keys: {Keys}", string.Join(", ", storageKeys));
            return Result.Failure(CloudinaryStorageErrors.BatchDeleteFailed);
        }
    }

    public async Task<bool> ExistsAsync(StorageKey storageKey, CancellationToken ct = default)
    {
        try
        {
            var publicId = BuildPublicId(storageKey);

            var result = await _cloudinary.GetResourceAsync(
                new GetResourceParams(publicId));

            return result.Error == null;
        }
        catch
        {
            return false;
        }
    }

    public string GetOptimizedUrl(StorageKey storageKey)
    {
        var publicId = BuildPublicId(storageKey);

        var url = _cloudinary.Api.UrlImgUp
            .Transform(new Transformation()
                .Quality("auto")
                .FetchFormat("auto"))
            .BuildUrl(publicId);

        return url;
    }
    
    public string GetCleanUrl(StorageKey storageKey)
    {
        var publicId = BuildPublicId(storageKey);

        var url = _cloudinary.Api.UrlImgUp
            .BuildUrl(publicId);

        return url;
    }

    private string BuildPublicId(StorageKey key)
    {
        var withoutExt = Path.GetFileNameWithoutExtension(key);
        var folder = Path.GetDirectoryName(key)?.Replace('\\', '/');

        var relativePath = string.IsNullOrEmpty(folder)
            ? withoutExt
            : $"{folder}/{withoutExt}";

        return string.IsNullOrEmpty(_options.UploadFolder)
            ? relativePath
            : $"{_options.UploadFolder}/{relativePath}";
    }
}

internal static class CloudinaryStorageErrors
{
    public static Shared.Error UploadFailed =>
        Shared.Error.Failure("CLOUDINARY_UPLOAD_FAILED", "Upload failed.");

    public static Shared.Error DeleteFailed =>
        Shared.Error.Failure("CLOUDINARY_DELETE_FAILED", "Delete failed.");

    public static Shared.Error BatchDeleteFailed =>
        Shared.Error.Failure("CLOUDINARY_BATCH_DELETE_FAILED", "Batch delete failed.");

    public static Shared.Error BatchDeleteFailedForKeys(IEnumerable<string> failedKeys)
        => Shared.Error.Failure(
            "CLOUDINARY_BATCH_DELETE_FAILED_FOR_KEYS",
            $"Delete failed for keys: {string.Join(", ", failedKeys)}");
}