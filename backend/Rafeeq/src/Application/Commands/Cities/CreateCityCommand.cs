using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Cities;

public sealed record CreateCityCommand(
    Stream Image,
    string OriginalFileName,
    string Name,
    string Description,
    double CenterLatitude,
    double CenterLongitude,
    int DisplayOrder) : ICommand;

internal sealed class CreateCityCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService) : ICommandHandler<CreateCityCommand>
{
    public async Task<Result> HandleAsync(CreateCityCommand command, CancellationToken cancellationToken)
    {
        var locationResult = GeoLocation.Create(command.CenterLatitude, command.CenterLongitude);
        if (locationResult.Failed)
            return locationResult;

        var imageStream = command.Image;
        
        try
        {   
            var ext = Path.GetExtension(command.OriginalFileName).ToLowerInvariant();
            var storageKey = StorageKey.ForCitiesImages(ext);

            imageStream.Position = 0;

            var uploadResult = await storageService.UploadAsync(
                imageStream,
                storageKey,
                cancellationToken);

            if (uploadResult.Failed)
            {
                return uploadResult;
            }

            var cityResult = City.Create(
                command.Name,
                command.Description,
                locationResult.Value,
                storageKey,
                uploadResult.Url,
                command.DisplayOrder);

            if (cityResult.Failed)
                return cityResult;

            await unitOfWork.Cities.AddAsync(cityResult.Value, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            return Result.Failure("An error occurred while uploading the image.");
        }

        return Result.Success();
    }
}

