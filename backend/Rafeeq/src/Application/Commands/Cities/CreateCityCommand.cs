using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Cities;

public record CreateCityCommand(
    string Name,
    string Description,
    GeoLocation CenterLocation) : ICommand;

internal class CreateCityCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCityCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> HandleAsync(CreateCityCommand command, CancellationToken cancellationToken)
    {
        var cityResult = City.Create(command.Name, command.Description, command.CenterLocation);
        if (cityResult.Failed)
            return cityResult;

        await _unitOfWork.Cities.AddAsync(cityResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

