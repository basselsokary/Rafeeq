using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Cities;

public record CreateCityCommand(
    string Name,
    string Description,
    double CenterLatitude,
    double CenterLongitude) : ICommand;

internal class CreateCityCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCityCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> HandleAsync(CreateCityCommand command, CancellationToken cancellationToken)
    {
        var locationResult = GeoLocation.Create(command.CenterLatitude, command.CenterLongitude);
        if (locationResult.Failed)
            return locationResult;

        var cityResult = City.Create(command.Name, command.Description, locationResult.Value);
        if (cityResult.Failed)
            return cityResult;

        await _unitOfWork.Cities.AddAsync(cityResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

