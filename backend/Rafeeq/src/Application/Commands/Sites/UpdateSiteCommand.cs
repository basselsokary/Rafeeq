using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites;

public sealed record UpdateSiteCommand(
    Guid Id,
    SiteType Type,
    double Latitude,
    double Longitude,
    int EstimatedDurationMinutes,
    decimal? EgyptianTicketPrice,
    decimal? ForeignerTicketPrice,
    string? ForeignerCurrency,
    bool IsFree,
    string? ContactPhone,
    string? ContactWebsiteUrl) : ICommand;

internal sealed class UpdateSiteCommandHandler(
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

        site.UpdateLocation(locationResult.Value);
        var result = site.UpdateBasicInfo(command.Type, command.EstimatedDurationMinutes);
        if (result.Failed)
            return result;
        
        if (!string.IsNullOrWhiteSpace(command.ContactPhone))
        {
            result = site.SetContactInfo(command.ContactPhone, command.ContactWebsiteUrl); 
            if (result.Failed)
                return result;
        }

        if (command.IsFree)
        {
            site.RemoveEntryFee(true);
            return Result.Success();
        }
        
        if (command.EgyptianTicketPrice == null && command.ForeignerTicketPrice == null)
        {   
            site.RemoveEntryFee();
        }
        else
        {
            var egyMoneyResult = Money.Create(command.EgyptianTicketPrice ?? -1 , "EGP");
            var foreMoneyResult = Money.Create(command.ForeignerTicketPrice ?? -1 , command.ForeignerCurrency ?? "USD");
            
            Result<Ticket> ticketResult;
            if (egyMoneyResult.Failed || foreMoneyResult.Failed)
            {
                return egyMoneyResult;
            }
            else if (foreMoneyResult.Failed)
                ticketResult = Ticket.Create(egyMoneyResult.Value, null);
            else
                ticketResult = Ticket.Create(egyMoneyResult.Value, foreMoneyResult.Value);

            if (ticketResult.Failed)
                return ticketResult;

            site.SetEntryFee(ticketResult.Value);
        }

        return Result.Success();
    }
}
