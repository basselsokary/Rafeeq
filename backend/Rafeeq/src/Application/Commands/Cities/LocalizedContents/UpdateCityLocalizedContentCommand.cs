using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.Enums;

namespace Application.Commands.Cities.LocalizedContents;

public sealed record UpdateCityLocalizedContentCommand(
    Guid Id,
    List<UpdateCityLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public sealed record UpdateCityLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Name,
    string Description);

internal sealed class UpdateCityLocalizedContentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCityLocalizedContentCommand>
{
    public async Task<Result> HandleAsync(UpdateCityLocalizedContentCommand command, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.Cities.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (city == null)
            return CityErrors.NotFound(command.Id);

        foreach (var content in command.LocalizedContents)
        {
            var result = city.UpdateLocalizedContent(content.Language, content.Name, content.Description);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}