using Application.Common.Interfaces.Services;
using Application.Common.Validators;
using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Cities;

public sealed record UpdateCityCommand(
    Guid Id,
    Stream? Image,
    string? OriginalFileName,
    double CenterLatitude,
    double CenterLongitude,
    int DisplayOrder) : ICommand;

internal sealed class UpdateCityCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService) : ICommandHandler<UpdateCityCommand>
{
    public async Task<Result> HandleAsync(UpdateCityCommand command, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.Cities.GetByIdAsync(command.Id, cancellationToken);
        if (city == null)
            return CityErrors.NotFound(command.Id);
        
        try
        {
            var result = await ApplyChanges(command, city, cancellationToken);
            if (result.Failed)
                return result;
        }
        catch
        {
            return Result.Failure(ImageErrors.UploadFailed);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> ApplyChanges(UpdateCityCommand command, City city, CancellationToken cancellationToken)
    {
        var locationResult = GeoLocation.Create(command.CenterLatitude, command.CenterLongitude);
        if (locationResult.Failed)
            return locationResult;
        
        city.SetCenterLocation(locationResult.Value);

        if (command.Image != null && command.OriginalFileName != null)
        {
            var ext = Path.GetExtension(command.OriginalFileName).ToLowerInvariant();
            if (!FileSignatureValidator.IsValid(command.Image, ext))
            {
                return ImageErrors.InvalidSignature;
            }

            var storageKey = StorageKey.ForCitiesImages(ext);

            command.Image.Position = 0;

            var uploadResult = await storageService.UploadAsync(
                command.Image,
                storageKey,
                cancellationToken);

            if (uploadResult.Failed)
            {
                return uploadResult;
            }

            city.SetImage(storageKey, uploadResult.Url);
        }

        var result = city.SetDisplayOrder(command.DisplayOrder);
        if (result.Failed)
            return result;

        return Result.Success();
    }
}

