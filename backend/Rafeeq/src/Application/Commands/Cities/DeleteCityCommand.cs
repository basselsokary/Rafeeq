using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;

namespace Application.Commands.Cities;

public sealed record DeleteCityCommand(Guid Id) : ICommand;

internal sealed class DeleteCityCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<DeleteCityCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> HandleAsync(DeleteCityCommand command, CancellationToken cancellationToken)
    {
        var city = await _unitOfWork.Cities.GetByIdAsync(command.Id, cancellationToken);
        if (city == null)
            return CityErrors.NotFound(command.Id);

        await _unitOfWork.Cities.DeleteAsync(city, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

