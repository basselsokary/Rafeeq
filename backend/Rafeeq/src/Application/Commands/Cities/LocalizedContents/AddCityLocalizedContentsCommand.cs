using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.Enums;

namespace Application.Commands.Cities.LocalizedContents;

public sealed record AddCityLocalizedContentsCommand(
    Guid Id,
    List<AddCityLocalizedContentsDtoCommand> LocalizedContents) : ICommand;

public sealed record AddCityLocalizedContentsDtoCommand(
    LanguageCode Language,
    string Name,
    string Description);

internal sealed class AddCityLocalizedContentsCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddCityLocalizedContentsCommand>
{
    public async Task<Result> HandleAsync(AddCityLocalizedContentsCommand command, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.Cities.GetWithLocalizedContentsAsync(command.Id, cancellationToken);
        if (city == null)
            return CityErrors.NotFound(command.Id);

        foreach (var content in command.LocalizedContents)
        {
            var result = city.AddLocalizedContent(content.Language, content.Name, content.Description);
            if (result.Failed)
                return result;
            
            await unitOfWork.AddAsync(result.Value, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

