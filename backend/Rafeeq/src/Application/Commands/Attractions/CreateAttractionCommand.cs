using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;
using Domain.Entities.SiteAggregate;
using Domain.Enums;

namespace Application.Commands.Attractions;

public record CreateAttractionCommand(
    Guid SiteId,
    string Name,
    string Description,
    AttractionType Type,
    HistoricalPeriod HistoricalPeriod) : ICommand;

internal class CreateAttractionCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateAttractionCommand>
{
    public async Task<Result> HandleAsync(CreateAttractionCommand command, CancellationToken cancellationToken)
    {
        var siteExist = await unitOfWork.Sites.AnyAsync(command.SiteId, cancellationToken);
        if (!siteExist)
            return SiteErrors.NotFound(command.SiteId);
            
        var attractionResult = Attraction.Create(
            command.SiteId,
            command.Name,
            command.Description,
            command.Type,
            command.HistoricalPeriod);

        if (attractionResult.Failed)
            return attractionResult;

        await unitOfWork.Attractions.AddAsync(attractionResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
