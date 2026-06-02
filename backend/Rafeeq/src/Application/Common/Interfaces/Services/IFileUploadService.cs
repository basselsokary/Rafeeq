using Application.DTOs.Services;
using Application.Services;

namespace Application.Common.Interfaces.Services;

public interface IFileUploadService
{
    Task<Result<FileUploadResponse<TMetadata>>> UploadSingleAsync<TMetadata>(
        UploadImageContext<TMetadata> file,
        Guid uploaderUserId,
        CancellationToken ct = default);

    Task<BatchUploadResult<TMetadata>> UploadMultipleAsync<TMetadata>(
        IReadOnlyList<UploadImageContext<TMetadata>> files,
        Guid uploaderUserId,
        CancellationToken ct = default);
}
