using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Application.DTOs.Services;
using Application.Services;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Sites.Images;

public sealed record AddSiteImagesCommand(
    Guid Id,
    List<AddSiteImageDto> Images) : ICommand<BatchUploadResult<ImageMetadata>>;

public sealed record AddSiteImageDto(
    FileUploadInput File,
    bool IsMain,
    int DisplayOrder,
    string? Caption = null);

internal sealed class AddSiteImagesCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService,
    IFileUploadService fileUploadService) : ICommandHandler<AddSiteImagesCommand, BatchUploadResult<ImageMetadata>>
{
    public async Task<Result<BatchUploadResult<ImageMetadata>>> HandleAsync(AddSiteImagesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithImagesAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);
        
        var failedUploadedKeys = new List<StorageKey>();

        var batchUploadResult = await fileUploadService.UploadMultipleAsync(
            command.Images.Select(i => new UploadImageContext<ImageMetadata>(
                i.File,
                new ImageMetadata(i.IsMain, i.DisplayOrder, i.Caption))).ToList(),
            Guid.Empty,
            cancellationToken);

        foreach (var successUpload in batchUploadResult.Succeeded)
        {
            var storageKey = successUpload.StorageKey;
            var imageUrl = successUpload.Url;
            var storedFileId = successUpload.FileId;

            var addImageResult = site.AddImage(
                storedFileId,
                storageKey,
                imageUrl,
                successUpload.Metadata.IsMain,
                successUpload.Metadata.DisplayOrder,
                successUpload.Metadata.Caption);

            if (addImageResult.Failed)
            {
                failedUploadedKeys.Add(storageKey);
                continue;
            }

            await unitOfWork.AddAsync(addImageResult.Value, cancellationToken);
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(batchUploadResult);
        }
        catch (Exception ex)
        {
            await FallBackStorageAsync(storageService, failedUploadedKeys, cancellationToken);
            return Result.Failure($"An error occurred while saving changes: {ex.Message}").To<BatchUploadResult<ImageMetadata>>();
        }
    }

    private static async Task FallBackStorageAsync(
        IFileStorageService storageService,
        List<StorageKey> uploadedKeys,
        CancellationToken cancellationToken)
    {
        await storageService.DeleteAsync(uploadedKeys, cancellationToken);
    }
}