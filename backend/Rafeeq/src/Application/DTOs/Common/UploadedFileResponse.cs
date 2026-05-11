using Domain.ValueObjects;

namespace Application.DTOs.Common;

public record UploadedFileResponse(
    StorageKey StorageKey,
    string Url,
    long Size = 0);