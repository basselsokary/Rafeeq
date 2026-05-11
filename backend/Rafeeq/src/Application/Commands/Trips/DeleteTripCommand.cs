using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TripAggregate;

namespace Application.Commands.Trips;

public sealed record DeleteTripCommand(Guid Id) : ICommand;

internal sealed class DeleteTripCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<DeleteTripCommand>
{
    public async Task<Result> HandleAsync(DeleteTripCommand command, CancellationToken cancellationToken)
    {
        var trip = await unitOfWork.Trips.GetByIdAsync(command.Id, cancellationToken);
        if (trip == null || trip.TouristId != userContext.Id)
            return TripErrors.NotFound;

        trip.Delete();
        await unitOfWork.Trips.DeleteAsync(trip, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
