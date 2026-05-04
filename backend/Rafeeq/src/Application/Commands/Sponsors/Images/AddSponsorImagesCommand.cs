using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors.Images;

public sealed record AddSponsorImagesCommand(
    Guid Id,
    List<AddSponsorImageDto> Images) : ICommand;

public sealed record AddSponsorImageDto(
    Stream Stream,
    string OriginalFileName,
    bool IsMain,
    int DisplayOrder,
    string? Caption = null);

internal sealed class AddSponsorImagesCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService) : ICommandHandler<AddSponsorImagesCommand>
{
    public async Task<Result> HandleAsync(AddSponsorImagesCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetWithImagesAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);

        var uploadedKeys = new List<StorageKey>();

        foreach (var image in command.Images)
        {
            try {
                var ext = Path.GetExtension(image.OriginalFileName).ToLowerInvariant();
                var storageKey = StorageKey.ForSponsorImages(ext);

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

                var addImageResult = sponsor.AddImage(
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
                return Result.Failure("An error occurred while processing the images.");
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
}