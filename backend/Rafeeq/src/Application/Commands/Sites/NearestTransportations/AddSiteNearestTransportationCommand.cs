using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites.NearestTransportations;

public sealed record AddSiteNearestTransportationCommand(
    Guid SiteId,
    TransportationType Type,
    double Latitude,
    double Longitude,
    double DistanceKm,
    List<AddNearestTransportationLocalizedContentDto> LocalizedContents) : ICommand;

public sealed record AddNearestTransportationLocalizedContentDto(LanguageCode Language, string Name, string? Description, string? Address);

internal sealed class AddSiteNearestTransportationCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteNearestTransportationCommand>
{
    public async Task<Result> HandleAsync(AddSiteNearestTransportationCommand command, CancellationToken cancellationToken = default)
    {
        var locationResult = GeoLocation.Create(command.Latitude, command.Longitude);
        if (locationResult.Failed)
            return locationResult;
    
        var site = await unitOfWork.Sites.GetWithNearestTransportationAsync(command.SiteId, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.SiteId);

        var result = site.AddNearestTransportation(
            command.Type,
            locationResult.Value,
            command.DistanceKm);

        if (result.Failed)
            return result;

        var transContentResult = AddNearestTransportationLocalizedContents(result.Value, command.LocalizedContents);
        if (transContentResult.Failed)
            return transContentResult;

        await unitOfWork.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Result AddNearestTransportationLocalizedContents(NearestTransportation value, List<AddNearestTransportationLocalizedContentDto> localizedContents)
    {
        foreach (var localizedContent in localizedContents)
        {
            Address? address = null;
            if (!string.IsNullOrWhiteSpace(localizedContent.Address))
            {
                var addressResult = Address.Create(localizedContent.Address);
                if (addressResult.Failed)
                    address = addressResult.Value;
            }

            var localizedContentResult = value.AddLocalizedContent(localizedContent.Language, localizedContent.Name, localizedContent.Description, address);
            if (localizedContentResult.Failed)
                return localizedContentResult;
        }

        return Result.Success();
    }
}