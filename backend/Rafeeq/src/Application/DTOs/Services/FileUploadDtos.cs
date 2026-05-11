using Domain.ValueObjects;

namespace Application.DTOs.Services;

public sealed record FileUploadResponse<TMetadata>(
    Guid FileId,
    string FileName,
    StorageKey StorageKey,
    ImageContentType ContentType,
    long SizeBytes,
    string Url,
    DateTimeOffset UploadedAt,
    string? Hash,
    bool IsReferenceAnotherEntity,
    TMetadata Metadata);

public sealed record BatchUploadResult<TMetadata>(
    IReadOnlyList<FileUploadResponse<TMetadata>> Succeeded,
    IReadOnlyList<FileUploadFailure<TMetadata>> Failed)
{
    public int TotalRequested => Succeeded.Count + Failed.Count;
}

public sealed record FileUploadFailure<TMetadata>(
    string OriginalFileName,
    string Reason,
    TMetadata Metadata);

internal sealed record UploadedFileMetadata(
    string StorageKey,
    string ContentType,
    long SizeBytes,
    string Url,
    string Hash);