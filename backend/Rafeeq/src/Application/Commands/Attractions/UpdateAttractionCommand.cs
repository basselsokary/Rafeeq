using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Attractions;

public sealed record UpdateAttractionCommand(
    Guid Id,
    AttractionType Type,
    List<HistoricalPeriod> HistoricalPeriods,
    double? Latitude,
    double? Longitude) : ICommand;

internal sealed class UpdateAttractionCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateAttractionCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> HandleAsync(UpdateAttractionCommand command, CancellationToken cancellationToken)
    {
        var attraction = await _unitOfWork.Attractions.GetByIdAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);
        
        Result result = UpdateAttractionLocation(command, attraction);
        if (result.Failed)
            return result;

        result = attraction.UpdateBasicInfo(
            command.Type,
            command.HistoricalPeriods);

        if (result.Failed)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Result UpdateAttractionLocation(UpdateAttractionCommand command, Attraction attraction)
    {
        if (command.Latitude.HasValue && command.Longitude.HasValue)
        {
            var locationResult = GeoLocation.Create(command.Latitude.Value, command.Longitude.Value);
            if (locationResult.Failed)
                return locationResult;

            attraction.SetLocation(locationResult.Value);
        }

        return Result.Success();
    }
}

