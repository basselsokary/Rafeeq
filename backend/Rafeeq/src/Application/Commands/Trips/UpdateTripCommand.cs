using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TripAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Trips;

public sealed record UpdateTripCommand(
    Guid Id,
    double Latitude,
    double Longitude,
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    decimal? EstimatedBudget,
    string Currency) : ICommand;

// internal sealed class UpdateTripCommandHandler(
//     IUnitOfWork unitOfWork,
//     IUserContext userContext) : ICommandHandler<UpdateTripCommand>
// {
//     public async Task<Result> HandleAsync(UpdateTripCommand command, CancellationToken cancellationToken)
//     {
//         var trip = await unitOfWork.Trips.GetByIdAsync(command.Id, cancellationToken);
//         if (trip == null || trip.TouristId != userContext.Id)
//             return TripErrors.NotFound;

//         var basicInfoResult = trip.UpdateBasicInfo(command.Name, command.Description);
//         if (basicInfoResult.Failed)
//             return basicInfoResult;

//         var dateRangeResult = DateRange.Create(command.StartDate, command.EndDate);
//         if (dateRangeResult.Failed)
//             return dateRangeResult;

//         trip.UpdateDateRange(dateRangeResult.Value);

//         var userPositionResult = GeoLocation.Create(command.Latitude, command.Longitude);
//         if (userPositionResult.Failed)
//             return userPositionResult;

//         trip.UpdateUserPosition(userPositionResult.Value);

//         Money? estimatedBudget = null;
//         if (command.EstimatedBudget.HasValue)
//         {
//             var estimatedBudgetResult = Money.Create(command.EstimatedBudget.Value, command.Currency);
//             if (estimatedBudgetResult.Failed)
//                 return estimatedBudgetResult;

//             estimatedBudget = estimatedBudgetResult.Value;
//         }

//         trip.UpdateEstimatedBudget(estimatedBudget);

//         await unitOfWork.Trips.UpdateAsync(trip, cancellationToken);
//         await unitOfWork.SaveChangesAsync(cancellationToken);

//         return Result.Success();
//     }
// }
