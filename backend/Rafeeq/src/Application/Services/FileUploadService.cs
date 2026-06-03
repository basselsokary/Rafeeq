using System.Security.Cryptography;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.Services;
using Application.Common.Validators;
using Application.DTOs.Services;
using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services;

public sealed record FileUploadInput(
    Stream Stream,
    string OriginalFileName,
    long Length);

public sealed record UploadImageContext<TMetadata>(
    FileUploadInput File,
    TMetadata Metadata = default!);

public sealed class FileUploadService(
    IFileStorageService storage,
    IOptions<FileUploadOptions> options,
    ILogger<FileUploadService> logger,
    IUnitOfWork unitOfWork,
    IErrorLocalizer errorLocalizer) : IFileUploadService
{
    private readonly FileUploadOptions options = options.Value;

    private sealed record PreparedUpload<T>(
        UploadImageContext<T> Context,
        string Ext,
        ImageContentType ContentType,
        FileHash Hash);

    private sealed record HashEntry(
        StoredFile StoredFile,
        string Url,
        bool IsExistingBeforeBatch);

    public async Task<Result<FileUploadResponse<T>>> UploadSingleAsync<T>(
        UploadImageContext<T> file,
        Guid uploaderUserId,
        CancellationToken ct = default)
    {
        var typeResult = ValidateAndResolveType(file.File);
        if (typeResult.Failed)
            return Result.Failure<FileUploadResponse<T>>(typeResult.Error);

        var (ext, contentType) = typeResult.Value;

        var hashResult = await ValidateSignatureAndComputeHashAsync(file.File, ext, ct);
        if (hashResult.Failed)
            return Result.Failure<FileUploadResponse<T>>(hashResult.Error);

        var hash = hashResult.Value;

        // Global dedup check across all image types
        if (options.EnableDuplicateDetection)
        {
            var existing = await unitOfWork.StoredFiles.FindByHashAsync(hash, ct);
            if (existing is not null)
            {
                logger.LogInformation(
                    "Duplicate detected for user {UserId}, hash {Hash} — reusing existing asset",
                    uploaderUserId, hash.Value);

                var existingUrl = storage.GetOptimizedUrl(existing.StorageKey);
                existing.IncrementReference();
                return Result.Success(MapToResponse(existing, existingUrl, file.Metadata));
            }
        }

        var uploadNewResult = await UploadNewAsync(file, uploaderUserId, hash, contentType, ext, ct);
        if (uploadNewResult.Failed)
            return Result.Failure<FileUploadResponse<T>>(uploadNewResult.Error);

        return Result.Success(uploadNewResult.Value.Response);
    }

    /// <summary>
    /// Uploads multiple files concurrently. Partial failures are captured
    /// individually — one bad file does NOT abort the entire batch.
    /// </summary>
    public async Task<BatchUploadResult<T>> UploadMultipleAsync<T>(
        IReadOnlyList<UploadImageContext<T>> files,
        Guid uploaderUserId,
        CancellationToken ct = default)
    {
        if (files.Count > options.MaxFilesPerRequest)
        {
            var failures = files.Select(f => new FileUploadFailure<T>(
                OriginalFileName: f.File.OriginalFileName,
                Reason: $"Batch exceeds maximum of {options.MaxFilesPerRequest} files.",
                Metadata: f.Metadata
            )).ToList();

            return new BatchUploadResult<T>(Succeeded: [], Failed: failures);
        }

        // IMPORTANT: EF Core DbContext isn't thread-safe. Running per-file uploads concurrently
        // causes concurrent DB operations (dedup checks + StoredFile adds) and can throw:
        // "A second operation was started on this context instance...".
        // We therefore precompute hashes, fetch duplicates once, then process sequentially.

        var prepared = new List<PreparedUpload<T>>(capacity: files.Count);
        var succeeded = new List<FileUploadResponse<T>>();
        var failed = new List<FileUploadFailure<T>>();

        foreach (var file in files)
        {
            try
            {
                var typeResult = ValidateAndResolveType(file.File);
                if (typeResult.Failed)
                {
                    failed.Add(new FileUploadFailure<T>(
                        OriginalFileName: file.File.OriginalFileName,
                        Reason: errorLocalizer[typeResult.Error.Code],
                        Metadata: file.Metadata));
                    continue;
                }

                var (ext, contentType) = typeResult.Value;

                var hashResult = await ValidateSignatureAndComputeHashAsync(file.File, ext, ct);
                if (hashResult.Failed)
                {
                    failed.Add(new FileUploadFailure<T>(
                        OriginalFileName: file.File.OriginalFileName,
                        Reason: errorLocalizer[hashResult.Error.Code],
                        Metadata: file.Metadata));
                    continue;
                }

                prepared.Add(new PreparedUpload<T>(
                    Context: file,
                    Ext: ext,
                    ContentType: contentType,
                    Hash: hashResult.Value));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while preparing upload for file {FileName}", file.File.OriginalFileName);
                failed.Add(new FileUploadFailure<T>(
                    OriginalFileName: file.File.OriginalFileName,
                    Reason: "An unexpected error occurred while preparing the file.",
                    Metadata: file.Metadata));
            }
        }

        var entriesByHash = new Dictionary<string, HashEntry>(StringComparer.OrdinalIgnoreCase);

        if (options.EnableDuplicateDetection && prepared.Count > 0)
        {
            var existingFiles = await unitOfWork.StoredFiles.FindByHashsAsync(
                prepared.Select(p => p.Hash).ToList(),
                ct);

            foreach (var existing in existingFiles)
            {
                logger.LogInformation(
                    "Duplicate detected for user {UserId}, hash {Hash} — reusing existing asset",
                    uploaderUserId, existing.Hash.Value);

                var url = storage.GetOptimizedUrl(existing.StorageKey);
                entriesByHash[existing.Hash.Value] = new HashEntry(
                    StoredFile: existing,
                    Url: url,
                    IsExistingBeforeBatch: true);
            }
        }

        var usageCountByHash = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in prepared)
        {
            var hashValue = item.Hash.Value;
            var usageCount = usageCountByHash.TryGetValue(hashValue, out var count)
                ? count + 1
                : 1;
            usageCountByHash[hashValue] = usageCount;

            if (entriesByHash.TryGetValue(hashValue, out var entry))
            {
                // Increment reference for any pre-existing stored file (each usage adds a reference).
                // For a newly-created stored file in this batch, we increment only on subsequent uses.
                if (entry.IsExistingBeforeBatch || usageCount > 1)
                    entry.StoredFile.IncrementReference();

                succeeded.Add(MapToResponse(entry.StoredFile, entry.Url, item.Context.Metadata));
                continue;
            }

            try
            {
                var uploadNewResult = await UploadNewAsync(
                    item.Context,
                    uploaderUserId,
                    item.Hash,
                    item.ContentType,
                    item.Ext,
                    ct);

                if (uploadNewResult.Failed)
                {
                    failed.Add(new FileUploadFailure<T>(
                        OriginalFileName: item.Context.File.OriginalFileName,
                        Reason: errorLocalizer[uploadNewResult.Error.Code],
                        Metadata: item.Context.Metadata));
                    continue;
                }

                var outcome = uploadNewResult.Value;
                entriesByHash[hashValue] = new HashEntry(
                    StoredFile: outcome.StoredFile,
                    Url: outcome.Response.Url,
                    IsExistingBeforeBatch: false);

                succeeded.Add(outcome.Response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while uploading file {FileName}", item.Context.File.OriginalFileName);
                failed.Add(new FileUploadFailure<T>(
                    OriginalFileName: item.Context.File.OriginalFileName,
                    Reason: "An unexpected error occurred while uploading the file.",
                    Metadata: item.Context.Metadata));
            }
        }

        if (failed.Count > 0)
            logger.LogWarning(
                "{FailCount}/{Total} files failed in batch upload by user {UserId}",
                failed.Count, files.Count, uploaderUserId);

        return new BatchUploadResult<T>(Succeeded: succeeded, Failed: failed);
    }

    private Result<(string Ext, ImageContentType ContentType)> ValidateAndResolveType(FileUploadInput file)
    {
        var ext = Path.GetExtension(file.OriginalFileName).ToLowerInvariant();

        var contentTypeResult = ImageContentType.FromExtension(ext);
        if (contentTypeResult.Failed)
            return Result.Failure<(string, ImageContentType)>(contentTypeResult.Error);

        // Validation: size and extension. Signature check is done later because it needs the stream.
        var validationResult = ValidateFile(file.Length, ext);
        if (validationResult.Failed)
            return Result.Failure<(string, ImageContentType)>(validationResult.Error);

        return Result.Success((ext, contentTypeResult.Value));
    }

    private static async Task<Result<FileHash>> ValidateSignatureAndComputeHashAsync(
        FileUploadInput file,
        string ext,
        CancellationToken ct)
    {
        file.Stream.Position = 0;
        if (!FileSignatureValidator.IsValid(file.Stream, ext))
            return Result.Failure<FileHash>(FileErrors.InvalidSignature);

        var hash = await ComputeSha256Async(file.Stream, ct);
        return Result.Success(hash);
    }

    private sealed record UploadOutcome<T>(StoredFile StoredFile, FileUploadResponse<T> Response);

    private async Task<Result<UploadOutcome<T>>> UploadNewAsync<T>(
        UploadImageContext<T> file,
        Guid uploaderUserId,
        FileHash hash,
        ImageContentType contentType,
        string ext,
        CancellationToken ct)
    {
        file.File.Stream.Position = 0;

        // Build storage key and upload
        var fileId = Guid.NewGuid();
        StorageKey storageKey;
        if (uploaderUserId == Guid.Empty)
        {
            storageKey = StorageKey.ForEntites(hash, ext);
        }
        else
        {
            storageKey = StorageKey.ForUserUpload(
                uploaderUserId,
                fileId,
                ext,
                DateTimeOffset.UtcNow);
        }

        var uploadResult = await storage.UploadAsync(file.File.Stream, storageKey, hash.Value, ct);
        if (uploadResult.Failed)
        {
            logger.LogError(
                "Storage upload failed for file {FileId}, user {UserId}",
                fileId,
                uploaderUserId);
            return Result.Failure<UploadOutcome<T>>(uploadResult.Error);
        }

        var storedFile = StoredFile.Create(
            fileId,
            hash,
            storageKey,
            (ImageContentType) contentType.Value,
            file.File.Length);

        await unitOfWork.StoredFiles.AddAsync(storedFile, ct);

        logger.LogInformation(
            "File {FileId} uploaded by user {UserId}. Size: {Size} bytes.",
            fileId,
            uploaderUserId,
            file.File.Length);

        var response = MapToResponse(storedFile, uploadResult.Value.Url, file.Metadata);
        return Result.Success(new UploadOutcome<T>(storedFile, response));
    }

    /// <summary>
    /// Synchronous validation — everything here is in-memory, no I/O needed.
    /// Signature check is done separately after this because it needs the stream.
    /// </summary>
    private Result ValidateFile(long length, string ext)
    {
        if (length == 0)
            return Result.Failure(FileErrors.EmptyFile);

        if (length > options.MaxFileSizeBytes)
            return Result.Failure(FileErrors.FileTooLarge(options.MaxFileSizeBytes));

        if (string.IsNullOrEmpty(ext) || !options.AllowedExtensions.Contains(ext))
            return Result.Failure(FileErrors.UnsupportedType(ext));

        return Result.Success();
    }

    private static async Task<FileHash> ComputeSha256Async(
        Stream stream, CancellationToken ct)
    {
        stream.Position = 0;
        var bytes = await SHA256.HashDataAsync(stream, ct);
        stream.Position = 0;
        return FileHash.From(Convert.ToHexString(bytes));
    }

    private static FileUploadResponse<T> MapToResponse<T>(StoredFile file, string url, T metadata) =>
        new(
            FileId: file.Id,
            FileName: file.Id.ToString(),
            StorageKey: (StorageKey) file.StorageKey.Value,
            ContentType: file.ContentType,
            SizeBytes: file.Size,
            Url: url,
            Hash: file.Hash.Value,
            UploadedAt: file.FirstUploadedAt,
            IsReferenceAnotherEntity: file.ReferenceCount > 1,
            Metadata: metadata);
}

public sealed class FileUploadOptions
{
    public const string SectionName = "FileUpload";

    public long MaxFileSizeBytes { get; init; } = 10 * 1024 * 1024; // 10 MB default
    public string[] AllowedExtensions { get; init; } = [".jpg", ".jpeg", ".png", ".webp"];
    public int MaxFilesPerRequest { get; init; } = 10;
    public bool EnableDuplicateDetection { get; init; } = true;
    public bool EnableImageResize { get; init; } = false;
    public int MaxWidthPx { get; init; } = 2048;
    public int MaxHeightPx { get; init; } = 2048;
}