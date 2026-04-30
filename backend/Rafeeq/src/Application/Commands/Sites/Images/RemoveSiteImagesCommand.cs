using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;

namespace Application.Commands.Sites.Images;

public sealed record RemoveSiteImagesCommand(
    Guid Id,
    List<Guid> ImageIds) : ICommand;

internal sealed class RemoveSiteImagesCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService) : ICommandHandler<RemoveSiteImagesCommand>
{
    public async Task<Result> HandleAsync(RemoveSiteImagesCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithImagesAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        var storageKeysToDelete = site.Images
            .Where(img => command.ImageIds.Contains(img.Id))
            .Select(img => img.StorageKey)
            .ToList();

        foreach (var imageId in command.ImageIds)
        {
            Result removeImageResult = site.RemoveImage(imageId);
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