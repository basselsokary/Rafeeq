using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors;

public record CreateSponsorCommand(
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

internal class CreateSponsorCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateSponsorCommand>
{
    public async Task<Result> HandleAsync(CreateSponsorCommand command, CancellationToken cancellationToken)
    {
        var locationResult = GeoLocation.Create(command.Latitude, command.Longitude);
        if (locationResult.Failed)
            return locationResult;

        var addressResult = Address.Create(command.Street, command.City, command.Region, command.PostalCode);
        if (addressResult.Failed)
            return addressResult;

        Result<Sponsor> result = Sponsor.Create(
            command.Title,
            command.Description,
            command.Type,
            command.Tier,
            locationResult.Value,
            addressResult.Value,
            command.StartDate,
            command.EndDate);
        
        if (result.Failed)
            return result;

        await unitOfWork.Sponsors.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}