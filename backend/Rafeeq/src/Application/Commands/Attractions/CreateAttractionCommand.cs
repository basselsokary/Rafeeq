using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Attractions;

public sealed record CreateAttractionCommand(
    Guid SiteId,
    string Name,
    string Description,
    string? LocationDescription,
    AttractionType Type,
    bool IsFeatured,
    double? Latitude,
    double? Longitude,
    List<HistoricalPeriod> HistoricalPeriod) : ICommand;

internal sealed class CreateAttractionCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateAttractionCommand>
{
    public async Task<Result> HandleAsync(CreateAttractionCommand command, CancellationToken cancellationToken)
    {
        var siteExist = await unitOfWork.Sites.AnyAsync(command.SiteId, cancellationToken);
        if (!siteExist)
            return SiteErrors.NotFound(command.SiteId);

        var attractionResult = Attraction.Create(
            command.SiteId,
            command.Name,
            command.Description,
            command.LocationDescription,
            command.Type,
            command.HistoricalPeriod);

        if (attractionResult.Failed)
            return attractionResult;
        
        var attraction = attractionResult.Value;
        
        GeoLocation? location = null;
        if (command.Latitude.HasValue && command.Longitude.HasValue)
        {
            var locationResult = GeoLocation.Create(command.Latitude.Value, command.Longitude.Value);
            if (locationResult.Failed)
                location = locationResult.Value;
        }
        
        attraction.SetLocation(location);
        attraction.SetAsFeatured(command.IsFeatured);

        await unitOfWork.Attractions.AddAsync(attraction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
