using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;

namespace Application.Commands.Cities.LocalizedContents;

public record UpdateCityLocalizedContentCommand(
    Guid Id,
    Guid ContentId,
    string Name,
    string Description) : ICommand;

internal class UpdateCityLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCityLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(UpdateCityLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.Cities.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (city == null)
            return CityErrors.NotFound(command.Id);

        Result result = city.UpdateLocalizedContent(command.ContentId, command.Name, command.Description);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}