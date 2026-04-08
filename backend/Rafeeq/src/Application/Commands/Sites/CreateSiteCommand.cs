using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites;

public record CreateSiteCommand(
    Guid CityId,
    string Name,
    string Description,
    SiteType Type,
    double Latitude,
    double Longitude,
    string Street,
    string City,
    string? Region,
    string? PostalCode) : ICommand;

internal class CreateSiteCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateSiteCommand>
{
    public async Task<Result> HandleAsync(CreateSiteCommand command, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.Cities.GetByIdAsync(command.CityId, cancellationToken);
        if (city == null)
            return CityErrors.NotFound(command.CityId);
        
        var locationResult = GeoLocation.Create(command.Latitude, command.Longitude);
        if (locationResult.Failed)
            return locationResult;

        var addressResult = Address.Create(command.Street, command.City, command.Region, command.PostalCode);
        if (addressResult.Failed)
            return addressResult;

        var siteResult = Site.Create(
            command.CityId,
            command.Name,
            command.Description,
            locationResult.Value,
            addressResult.Value,
            command.Type);

        if (siteResult.Failed)
            return siteResult;

        city.IncrementSiteCount();

        await unitOfWork.Sites.AddAsync(siteResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
