using Domain.Common.Interfaces;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sponsors;

public sealed record UpdateSponsorCommand(
    Guid Id,
    SponsorType Type,
    SponsorTier Tier,
    double Latitude,
    double Longitude,
    DateTime? NewEndDate,
    string? ContactPhone,
    string? ContactEmail,
    string? WebsiteUrl) : ICommand;

internal sealed class UpdateSponsorCommandHandler(
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

        sponsor.UpdateTier(command.Tier);
        sponsor.UpdateLocation(locationResult.Value);
        Result result = sponsor.UpdateBasicInfo(command.Type);
        if (result.Failed)
            return result;
        
        if (command.NewEndDate is not null)
        {
            result = sponsor.ExtendContract(command.NewEndDate.Value);
            if (result.Failed)
                return result;
        }
        
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

        sponsor.SetContactInfo(phone, email, command.WebsiteUrl);

        return Result.Success();
    }
}