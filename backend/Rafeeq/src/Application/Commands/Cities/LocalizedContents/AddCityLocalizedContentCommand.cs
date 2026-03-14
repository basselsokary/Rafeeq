using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.Enums;

namespace Application.Commands.Cities.LocalizedContents;

public record AddCityLocalizedContentCommand(
    Guid Id,
    LanguageCode Language,
    string Name,
    string Description) : ICommand;

internal class AddCityLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddCityLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(AddCityLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.Cities.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (city == null)
            return CityErrors.NotFound(command.Id);

        Result result = city.AddLocalizedContent(command.Language, command.Name, command.Description);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

