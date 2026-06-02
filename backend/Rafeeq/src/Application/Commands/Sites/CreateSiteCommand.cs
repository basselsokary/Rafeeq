using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites;

public sealed record CreateSiteCommand(
    Guid CityId,
    string Name,
    string Description,
    string Address,
    SiteType Type,
    double Latitude,
    double Longitude,
    int EstimatedDurationMinutes,
    decimal? EgyptianTicketPrice,
    decimal? ForeignerTicketPrice,
    string? ForeignerCurrency,
    string? EntryFreeNotes,
    bool IsFree,
    string? ContactPhone,
    string? ContactWebsiteUrl) : ICommand<Guid>;

public sealed record AddSiteLocalizedContentDto(LanguageCode Language, string Name, string Description, string Address, string? EntryFreeNotes);

internal sealed class CreateSiteCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateSiteCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateSiteCommand command, CancellationToken cancellationToken)
    {
        var locationResult = GeoLocation.Create(command.Latitude, command.Longitude);
        if (locationResult.Failed)
            return locationResult.Error;
        
        var addressResult = Address.Create(command.Address);
        if (addressResult.Failed)
            return addressResult.Error;
        
        var siteResult = Site.Create(
            command.CityId,
            command.Name,
            command.Description,
            addressResult.Value,
            command.EntryFreeNotes,
            locationResult.Value,
            command.Type,
            command.EstimatedDurationMinutes,
            command.ContactPhone,
            command.ContactWebsiteUrl);
        
        if (siteResult.Failed)
            return siteResult.Error;

        var site = siteResult.Value;
        var ticketResult = AddTicket(site, command);
        if (ticketResult.Failed)
            return ticketResult.Error;

        var city = await unitOfWork.Cities.GetByIdAsync(
            command.CityId,
            cancellationToken);

        if (city == null)
            return CityErrors.NotFound(command.CityId);

        city.IncrementSiteCount();
        
        await unitOfWork.Sites.AddAsync(siteResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return site.Id;
    }

    private static Result AddTicket(Site site, CreateSiteCommand command)
    {
        if (command.IsFree)
        {
            site.RemoveEntryFee(true);
            return Result.Success();
        }

        if (command.EgyptianTicketPrice == null && command.ForeignerTicketPrice == null)
        {
            site.RemoveEntryFee();
            return Result.Success().To<Ticket>();
        }

        var egyMoneyResult = Money.Create(command.EgyptianTicketPrice ?? -1 , "EGP");
        var foreMoneyResult = Money.Create(command.ForeignerTicketPrice ?? -1 , command.ForeignerCurrency ?? "USD");
        
        Result<Ticket> ticketResult;
        if (egyMoneyResult.Failed || foreMoneyResult.Failed)
            return egyMoneyResult;
        else if (foreMoneyResult.Failed)
            ticketResult = Ticket.Create(egyMoneyResult.Value, null);
        else
            ticketResult = Ticket.Create(egyMoneyResult.Value, foreMoneyResult.Value);
        
        if (ticketResult.Failed)
            return ticketResult;

        site.SetEntryFee(ticketResult.Value);
        return Result.Success();
    }
}
