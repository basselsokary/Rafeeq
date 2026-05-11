using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Cities;

public sealed record UpdateCityCommand(
    Guid Id,
    double CenterLatitude,
    double CenterLongitude,
    int DisplayOrder) : ICommand;

internal sealed class UpdateCityCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCityCommand>
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

        var result = city.SetDisplayOrder(command.DisplayOrder);
        if (result.Failed)
            return result;

        return Result.Success();
    }
}

