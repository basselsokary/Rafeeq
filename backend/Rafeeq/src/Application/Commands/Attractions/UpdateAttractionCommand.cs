using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Attractions;

public record UpdateAttractionCommand(
    Guid Id,
    string Name,
    string Description,
    AttractionType Type,
    HistoricalPeriod HistoricalPeriod,
    GeoLocation? ExactLocation,
    string? LocationDescription) : ICommand;

internal class UpdateAttractionCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateAttractionCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> HandleAsync(UpdateAttractionCommand command, CancellationToken cancellationToken)
    {
        var attraction = await _unitOfWork.Attractions.GetByIdAsync(command.Id, cancellationToken);
        if (attraction == null)
            return AttractionErrors.NotFound(command.Id);

        attraction.SetLocation(command.ExactLocation, command.LocationDescription);
        var attractionResult = attraction.UpdateBasicInfo(
            command.Name,
            command.Description,
            command.Type,
            command.HistoricalPeriod);

        if (attractionResult.Failed)
            return attractionResult;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

