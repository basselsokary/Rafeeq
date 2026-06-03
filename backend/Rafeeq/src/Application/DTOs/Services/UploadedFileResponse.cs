using Domain.ValueObjects;

namespace Application.DTOs.Services;

public record UploadedFileResponse(
    StorageKey StorageKey,
    string Url,
    long Size = 0);