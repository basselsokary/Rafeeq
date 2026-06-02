using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Application.Services;
using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Cities;

public sealed record CreateCityCommand(
    FileUploadInput File,
    string Name,
    string Description,
    double CenterLatitude,
    double CenterLongitude,
    int DisplayOrder) : ICommand<Guid>;

internal sealed class CreateCityCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService storageService,
    IFileUploadService fileUploadService) : ICommandHandler<CreateCityCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateCityCommand command, CancellationToken cancellationToken)
    {
        var locationResult = GeoLocation.Create(command.CenterLatitude, command.CenterLongitude);
        if (locationResult.Failed)
            return locationResult.Error;
        
        var uploadResult = await fileUploadService.UploadSingleAsync(new UploadImageContext<ImageMetadata>(command.File), Guid.Empty, cancellationToken);

        if (uploadResult.Failed)
            return uploadResult.Error;

        var cityResult = City.Create(
            uploadResult.Value.FileId,
            command.Name,
            command.Description,
            locationResult.Value,
            uploadResult.Value.StorageKey,
            uploadResult.Value.Url,
            command.DisplayOrder);

        if (cityResult.Failed)
        {
            await storageService.DeleteAsync(uploadResult.Value.StorageKey, cancellationToken);
            return cityResult.Error;
        }

        await unitOfWork.Cities.AddAsync(cityResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return cityResult.Value.Id;
    }
}

