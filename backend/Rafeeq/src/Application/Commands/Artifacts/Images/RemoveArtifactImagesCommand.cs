using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.ArtifactAggregate;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Artifacts.Images;

public sealed record RemoveArtifactImagesCommand(
    Guid Id,
    List<Guid> ImageIds) : ICommand;

internal sealed class RemoveArtifactImagesCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService,
    ILogger<RemoveArtifactImagesCommandHandler> logger) : ICommandHandler<RemoveArtifactImagesCommand>
{
    public async Task<Result> HandleAsync(RemoveArtifactImagesCommand command, CancellationToken cancellationToken)
    {
        var imageIds = command.ImageIds
            .Distinct()
            .ToList();

        var storageKeysToDelete = new List<StorageKey>();

        var dbResult = await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var artifact = await unitOfWork.Artifacts.GetWithImagesAsync(command.Id, cancellationToken);
            if (artifact == null)
                return ArtifactErrors.NotFound(command.Id.ToString());

            foreach (var imageId in imageIds)
            {
                var removeImageResult = artifact.RemoveImage(imageId);
                if (removeImageResult.Failed)
                    return removeImageResult;

                storageKeysToDelete.Add(removeImageResult.Value.StorageKey);
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
                "Failed to delete {Count} storage blobs after removing artifact images for artifact {ArtifactId}. Error: {Error}",
                storageKeysToDelete.Count,
                command.Id,
                deleteResult.Error);
        }

        return Result.Success();
    }
}
