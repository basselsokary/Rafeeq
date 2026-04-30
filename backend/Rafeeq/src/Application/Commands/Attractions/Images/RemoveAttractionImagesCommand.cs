using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;

namespace Application.Commands.Attractions.Images;

public sealed record RemoveAttractionImagesCommand(
    Guid Id,
    List<Guid> ImageIds) : ICommand;

internal sealed class RemoveAttractionImagesCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService) : ICommandHandler<RemoveAttractionImagesCommand>
{

    public async Task<Result> HandleAsync(RemoveAttractionImagesCommand command, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Attractions.GetWithImagesAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        var storageKeysToDelete = attraction.Images
            .Where(img => command.ImageIds.Contains(img.Id))
            .Select(img => img.StorageKey)
            .ToList();

        foreach (var imageId in command.ImageIds)
        {
            Result removeImageResult = attraction.RemoveImage(imageId);
            if (removeImageResult.Failed)
                return removeImageResult;
        }

        var result = await storageService.DeleteAsync(storageKeysToDelete, cancellationToken);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}