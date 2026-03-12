using Application.Common.Interfaces.Messaging;
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
    GeoLocation Location,
    Address Address,
    SiteType Type
) : ICommand;

internal class CreateSiteCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateSiteCommand>
{
    public async Task<Result> HandleAsync(CreateSiteCommand command, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.Cities.GetByIdAsync(command.CityId, cancellationToken);
        if (city == null)
            return CityErrors.NotFound(command.CityId);
        
        var siteResult = Site.Create(
            command.CityId,
            command.Name,
            command.Description,
            command.Location,
            command.Address,
            command.Type);

        if (siteResult.Failed)
            return siteResult;

        city.IncrementSiteCount();

        await unitOfWork.Sites.AddAsync(siteResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
