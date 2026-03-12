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
    GeoLocation Location,
    Address Address,
    Money? Fee) : ICommand;

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
        site.UpdateLocation(command.Location, command.Address);

        if (command.Fee == null)
            site.RemoveEntryFee();
        else
            site.SetEntryFee(command.Fee);

        var siteResult = site.UpdateBasicInfo(command.Name, command.Description, command.Type);
        if (siteResult.Failed)
            return siteResult;
        
        return Result.Success();
    }
}
