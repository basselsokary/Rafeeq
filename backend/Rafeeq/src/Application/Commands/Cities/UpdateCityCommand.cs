using Application.Common.Interfaces.Messaging;
using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Cities;

public record UpdateCityCommand(
    Guid Id,
    string Name,
    string Description,
    GeoLocation CenterLocation,
    int DisplayOrder,
    string? ImageUrl) : ICommand;

internal class UpdateCityCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCityCommand>
{
    public async Task<Result> HandleAsync(UpdateCityCommand command, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.Cities.GetByIdAsync(command.Id, cancellationToken);
        if (city == null)
            return CityErrors.NotFound(command.Id);
        
        var result = ApplyChanges(command, city);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Result ApplyChanges(UpdateCityCommand command, City city)
    {
        city.SetCenterLocation(command.CenterLocation);
        var cityResult = city.UpdateBasicInfo(command.Name, command.Description);
        if (cityResult.Failed)
            return cityResult;

        if (command.ImageUrl != null)
        {
            cityResult = city.SetImage(command.ImageUrl);
            if (cityResult.Failed)
                return cityResult;
        }

        cityResult = city.SetDisplayOrder(command.DisplayOrder);
        if (cityResult.Failed)
            return cityResult;

        return Result.Success();
    }
}

