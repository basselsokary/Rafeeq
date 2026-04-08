using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors;

public record UpdateSponsorCommand(
    Guid Id,
    string Title,
    string Description,
    SponsorType Type,
    SponsorTier Tier,
    double Latitude,
    double Longitude,
    string Street,
    string City,
    string? Region,
    string? PostalCode,
    DateTime StartDate,
    DateTime EndDate) : ICommand;

internal class UpdateSponsorCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSponsorCommand>
{
    public async Task<Result> HandleAsync(UpdateSponsorCommand command, CancellationToken cancellationToken)
    {
        var sponsor = await unitOfWork.Sponsors.GetByIdAsync(command.Id, cancellationToken);
        if (sponsor == null)
            return SponsorErrors.NotFound(command.Id);
        
        Result result = ApplyChanges(command, sponsor);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Result ApplyChanges(UpdateSponsorCommand command, Sponsor sponsor)
    {
        var locationResult = GeoLocation.Create(command.Latitude, command.Longitude);
        if (locationResult.Failed)
            return locationResult;

        var addressResult = Address.Create(command.Street, command.City, command.Region, command.PostalCode);
        if (addressResult.Failed)
            return addressResult;

        sponsor.UpdateTier(command.Tier);
        sponsor.UpdateLocation(locationResult.Value, addressResult.Value);
        Result result = sponsor.UpdateBasicInfo(command.Title, command.Description, command.Type);
        if (result.Failed)
            return result;

        return Result.Success();
    }
}