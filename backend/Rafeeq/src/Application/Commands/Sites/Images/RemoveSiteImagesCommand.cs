using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Sites.Images;

public sealed record RemoveSiteImagesCommand(
    Guid Id,
    List<Guid> ImageIds) : ICommand;

internal sealed class RemoveSiteImagesCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService,
    ILogger<RemoveSiteImagesCommandHandler> logger) : ICommandHandler<RemoveSiteImagesCommand>
{
    public async Task<Result> HandleAsync(RemoveSiteImagesCommand command, CancellationToken cancellationToken)
    {
        var imageIds = command.ImageIds
            .Distinct()
            .ToList();

        var storageKeysToDelete = new List<StorageKey>();

        var dbResult = await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var site = await unitOfWork.Sites.GetWithImagesAsync(command.Id, cancellationToken);
            if (site == null)
                return SiteErrors.NotFound(command.Id);

            var removedImages = new List<SiteImage>(capacity: imageIds.Count);
            foreach (var imageId in imageIds)
            {
                var removeImageResult = site.RemoveImage(imageId);
                if (removeImageResult.Failed)
                    return removeImageResult;

                removedImages.Add(removeImageResult.Value);
            }

            var storedFileIds = removedImages
                .Select(img => img.StoredFileId)
                .Distinct()
                .ToList();

            if (storedFileIds.Count == 0)
                return Result.Success();

            var storedFiles = await unitOfWork.StoredFiles.GetByIdsAsync(storedFileIds, cancellationToken);
            var storedFilesById = storedFiles.ToDictionary(sf => sf.Id);

            var removedCountByStoredFileId = removedImages
                .GroupBy(i => i.StoredFileId)
                .ToDictionary(g => g.Key, g => g.Count());

            // Decrement reference counts and collect storage keys to delete if reference count reaches zero
            foreach (var kvp in removedCountByStoredFileId)
            {
                var storedFileId = kvp.Key;
                var removeCount = kvp.Value;

                if (!storedFilesById.TryGetValue(storedFileId, out var storedFile))
                    return Result.Failure($"Stored file {storedFileId} was not found.");

                for (var i = 0; i < removeCount; i++)
                    storedFile.DecrementReference();

                if (storedFile.ReferenceCount <= 0)
                {
                    storageKeysToDelete.Add(storedFile.StorageKey);
                    await unitOfWork.DeleteAsync(storedFile);
                }
            }

            return Result.Success();
        }, cancellationToken);

        if (dbResult.Failed)
            return dbResult;

        if (storageKeysToDelete.Count == 0)
            return Result.Success();

        var deleteResult = await storageService.DeleteAsync(
            storageKeysToDelete.Distinct(),
            cancellationToken);
        
        if (deleteResult.Failed)
        {
            logger.LogWarning(
                "Failed to delete {Count} storage blobs after removing site images for site {SiteId}. Error: {Error}",
                storageKeysToDelete.Count,
                command.Id,
                deleteResult.Error);
        }

        return Result.Success();
    }
}