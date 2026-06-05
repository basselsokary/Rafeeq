using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Application.DTOs.Services;
using Application.Services;
using Domain.Common.Interfaces;
using Domain.Entities.ArtifactAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Artifacts.Images;

public sealed record AddArtifactImagesCommand(
    Guid Id,
    List<AddArtifactImageDto> Images) : ICommand;

public sealed record AddArtifactImageDto(
    FileUploadInput File,
    bool IsMain,
    int DisplayOrder,
    string? Caption = null);

internal sealed class AddArtifactImagesCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService,
    IFileUploadService fileUploadService) : ICommandHandler<AddArtifactImagesCommand>
{
    public async Task<Result> HandleAsync(AddArtifactImagesCommand command, CancellationToken cancellationToken)
    {
        var artifact = await unitOfWork.Artifacts.GetWithImagesAsync(command.Id, cancellationToken);
        if (artifact == null)
            return ArtifactErrors.NotFound(command.Id.ToString());

        var failedUploadedKeys = new List<StorageKey>();

        var batchUploadResult = await fileUploadService.UploadMultipleAsync(
            command.Images.Select(i => new UploadImageContext<ImageMetadata>(
                i.File,
                new ImageMetadata(i.IsMain, i.DisplayOrder, i.Caption))).ToList(),
            Guid.Empty,
            cancellationToken);

        foreach (var success in batchUploadResult.Succeeded)
        {
            var storageKey = success.StorageKey;
            var imageUrl = success.Url;

            var addImageResult = artifact.AddImage(
                storageKey,
                imageUrl,
                success.Metadata.IsMain,
                success.Metadata.DisplayOrder,
                success.Metadata.Caption);

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
