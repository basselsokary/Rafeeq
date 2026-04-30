using Application.Common.Interfaces.Services;
using Application.Common.Models;
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
    private readonly CloudinarySettings _options;
    private readonly ILogger<CloudinaryStorageService> _logger;

    public CloudinaryStorageService(
        IOptions<CloudinarySettings> options,
        ILogger<CloudinaryStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;

        var account = new Account(_options.CloudName, _options.ApiKey, _options.ApiSecret);
        _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
    }

    public async Task<UploadedFileResult> UploadAsync(
        Stream stream,
        StorageKey storageKey,
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
            };

            var result = await _cloudinary.UploadAsync(uploadParams, ct);

            if (result.Error != null)
            {
                _logger.LogError("Upload failed: {Message}", result.Error.Message);
                return (UploadedFileResult)Result.Failure("Upload failed.");
            }

            _logger.LogInformation("Upload succeeded: {Key} -> {Url}", storageKey, result.SecureUrl);

            return UploadedFileResult.Success(storageKey, result.SecureUrl.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed for {Key}", storageKey);
            return (UploadedFileResult)Result.Failure("Upload failed.");
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
                return Result.Failure("Delete failed.");
            
            _logger.LogInformation("Delete succeeded: {Key}", storageKey);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete failed for {Key}", storageKey);
            return Result.Failure("Delete failed.");
        }
    }

    public async Task<Result> DeleteAsync(IEnumerable<StorageKey> storageKeys, CancellationToken ct = default)
    {
        try
        {
            var publicIds = storageKeys.Select(BuildPublicId).ToArray();

            var result = await _cloudinary.DeleteResourcesAsync(publicIds);

            if (result.Deleted.All(kv => kv.Value is "ok" or "not found"))
            {
                _logger.LogInformation("Batch delete succeeded: {Keys}", string.Join(", ", storageKeys));
                return Result.Success();
            }
            else
            {
                var failedKeys = result.Deleted.Where(kv => kv.Value != "ok" && kv.Value != "not found")
                    .Select(kv => kv.Key);
                _logger.LogError("Batch delete partially failed for keys: {Keys}", string.Join(", ", failedKeys));
                return Result.Failure($"Delete failed for keys: {string.Join(", ", failedKeys)}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Batch delete failed for keys: {Keys}", string.Join(", ", storageKeys));
            return Result.Failure("Batch delete failed.");
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

    public Task<string> GetUrlAsync(StorageKey storageKey, CancellationToken ct = default)
    {
        var publicId = BuildPublicId(storageKey);

        var url = _cloudinary.Api.UrlImgUp
            .Transform(new Transformation()
                .Quality("auto")
                .FetchFormat("auto"))
            .BuildUrl(publicId);

        return Task.FromResult(url);
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