using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites;

public record UpdateSiteCommand(
    Guid Id,
    string Name,
    string Description,
    SiteType Type,
    double Latitude,
    double Longitude,
    string Street,
    string City,
    string? Region,
    string? PostalCode,
    decimal? Fee) : ICommand;

internal class UpdateSiteCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSiteCommand>
{
    public async Task<Result> HandleAsync(UpdateSiteCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);
        
        Result result = ApplyChanges(command, site);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Result ApplyChanges(UpdateSiteCommand command, Site site)
    {
        var locationResult = GeoLocation.Create(command.Latitude, command.Longitude);
        if (locationResult.Failed)
            return locationResult;
        
        var addressResult = Address.Create(command.Street, command.City, command.Region, command.PostalCode);
        if (addressResult.Failed)
            return addressResult;

        site.UpdateLocation(locationResult.Value, addressResult.Value);

        if (command.Fee == null)
        {   
            site.RemoveEntryFee();
        }
        else
        {
            var moneyResult = Money.Create(command.Fee.Value);
            if (moneyResult.Failed)
                return moneyResult;
            
            site.SetEntryFee(moneyResult.Value);
        }

        var siteResult = site.UpdateBasicInfo(command.Name, command.Description, command.Type);
        if (siteResult.Failed)
            return siteResult;
        
        return Result.Success();
    }
}
