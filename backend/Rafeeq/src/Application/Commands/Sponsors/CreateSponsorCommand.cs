using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors;

public sealed record CreateSponsorCommand(
    string Title,
    string Description,
    string Address,
    SponsorType Type,
    SponsorTier Tier,
    double Latitude,
    double Longitude,
    DateTime StartDate,
    DateTime EndDate,
    string? WebsiteUrl,
    string? ContactPhone,
    string? ContactEmail) : ICommand;

internal sealed class CreateSponsorCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateSponsorCommand>
{
    public async Task<Result> HandleAsync(CreateSponsorCommand command, CancellationToken cancellationToken)
    {
        var locationResult = GeoLocation.Create(command.Latitude, command.Longitude);
        if (locationResult.Failed)
            return locationResult;

        var dateRange = DateRange.Create(command.StartDate, command.EndDate);
        if (dateRange.Failed)
            return dateRange;
        
        var addressResult = Address.Create(command.Address);
        if (addressResult.Failed)
            return addressResult;
        
        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(command.ContactPhone))
        {
            var phoneResult = PhoneNumber.Create(command.ContactPhone);
            if (phoneResult.Succeeded)
                phone = phoneResult.Value;
        }

        Email? email = null;
        if (!string.IsNullOrWhiteSpace(command.ContactEmail))
        {
            var emailResult = Email.Create(command.ContactEmail);
            if (emailResult.Succeeded)
                email = emailResult.Value;
        }

        Result<Sponsor> sponsorResult = Sponsor.Create(
            command.Title,
            command.Description,
            addressResult.Value,
            command.Type,
            command.Tier,
            locationResult.Value,
            dateRange.Value,
            command.WebsiteUrl,
            phone,
            email);

        if (sponsorResult.Failed)
            return sponsorResult;

        await unitOfWork.Sponsors.AddAsync(sponsorResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}