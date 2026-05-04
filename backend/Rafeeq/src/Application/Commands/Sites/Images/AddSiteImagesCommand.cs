using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sites.Images;

public sealed record AddSiteImagesCommand(
    Guid Id,
    List<AddSiteImageDto> Images) : ICommand;

public sealed record AddSiteImageDto(
    Stream Stream,
    string OriginalFileName,
    bool IsMain,
    int DisplayOrder,
    string? Caption = null);

internal sealed class AddSiteImagesCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService) : ICommandHandler<AddSiteImagesCommand>
{
    public async Task<Result> HandleAsync(AddSiteImagesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithImagesAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);
        
        var uploadedKeys = new List<StorageKey>();

        foreach (var image in command.Images)
        {
            try {
                var ext = Path.GetExtension(image.OriginalFileName).ToLowerInvariant();
                var storageKey = StorageKey.ForSiteImages(ext);

                image.Stream.Position = 0;

                var uploadResult = await storageService.UploadAsync(
                    image.Stream,
                    storageKey,
                    cancellationToken);

                if (uploadResult.Failed)
                {
                    await FallBackStorageAsync(storageService, uploadedKeys, cancellationToken);
                    return uploadResult;
                }

                var addImageResult = site.AddImage(
                    storageKey,         // Store key (important!)
                    uploadResult.Url, // URL
                    image.IsMain,
                    image.DisplayOrder,
                    image.Caption);

                if (addImageResult.Failed)
                {
                    await FallBackStorageAsync(storageService, uploadedKeys, cancellationToken);
                    return addImageResult;
                }

                uploadedKeys.Add(storageKey);
                await unitOfWork.AddAsync(addImageResult.Value, cancellationToken);
            }
            catch
            {
                await FallBackStorageAsync(storageService, uploadedKeys, cancellationToken);
                return Result.Failure("An error occurred while uploading the images.");
            }
            finally
            {
                image.Stream.Close();
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static async Task FallBackStorageAsync(
        IFileStorageService storageService,
        List<StorageKey> uploadedKeys,
        CancellationToken cancellationToken)
    {
        await storageService.DeleteAsync(uploadedKeys, cancellationToken);
    }

    // private static async Task<FileHash> ComputeHashAsync(
    //     Stream stream, CancellationToken ct)
    // {
    //     stream.Position = 0;
    //     var bytes = await SHA256.HashDataAsync(stream, ct);
    //     stream.Position = 0;
    //     return FileHash.From(Convert.ToHexString(bytes));
    // }

    // private static FileUploadResponse MapToResponse(UploadedFile file, string url) =>
    //     new(
    //         FileId:      file.Id.ToString(),
    //         FileName:    file.FileName.Value,
    //         ContentType: file.ContentType.Value,
    //         SizeBytes:   file.Size.Bytes,
    //         Url:         url,
    //         Hash:        file.Hash.Value,
    //         UploadedAt:  file.UploadedAt);
}