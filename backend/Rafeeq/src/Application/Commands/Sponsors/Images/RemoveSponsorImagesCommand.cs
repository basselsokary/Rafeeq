using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;

namespace Application.Commands.Sponsors.Images;

public sealed record RemoveSponsorImagesCommand(
    Guid Id,
    List<Guid> ImageIds) : ICommand;

internal sealed class RemoveSponsorImagesCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService) : ICommandHandler<RemoveSponsorImagesCommand>
{
    public async Task<Result> HandleAsync(RemoveSponsorImagesCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetWithImagesAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);

        var storageKeysToDelete = sponsor.Images
            .Where(img => command.ImageIds.Contains(img.Id))
            .Select(img => img.StorageKey)
            .ToList();

        foreach (var imageId in command.ImageIds)
        {
            Result removeImageResult = sponsor.RemoveImage(imageId);
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