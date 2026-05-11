using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Application.Services;
using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;

namespace Application.Commands.Cities;

public sealed record SetCityImageCommand(
    Guid CityId,
    FileUploadInput File) : ICommand;

internal sealed class SetCityImageCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService,
    IFileUploadService fileUploadService) : ICommandHandler<SetCityImageCommand>
{
    public async Task<Result> HandleAsync(SetCityImageCommand command, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.Cities.GetByIdAsync(command.CityId, cancellationToken);
        if (city is null)
            return CityErrors.NotFound(command.CityId);

        var uploadResult = await fileUploadService.UploadSingleAsync(
            new UploadImageContext<ImageMetadata>(command.File),
            Guid.Empty,
            cancellationToken);

        if (uploadResult.Failed)
        {
            return uploadResult;
        }

        var uploadedFile = uploadResult.Value;

        var oldImageKey = city.StorageKey;

        var result = city.SetImage(uploadedFile.FileId, uploadedFile.StorageKey, uploadedFile.Url);
        if (result.Failed)
        {
            await storageService.DeleteAsync(uploadedFile.StorageKey, cancellationToken);
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (oldImageKey is not null && !uploadedFile.IsReferenceAnotherEntity)
        {
            await storageService.DeleteAsync(oldImageKey, cancellationToken);
        }

        return Result.Success();
    }
}