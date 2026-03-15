using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;
using Domain.Enums;

namespace Application.Commands.Users.Tourists;

public record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string Nationality,
    LanguageCode PreferredLanguage) : ICommand;

internal class UpdateProfileCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<UpdateProfileCommand>
{
    public async Task<Result> HandleAsync(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var tourist = await unitOfWork.Tourists.GetByIdAsync(userContext.Id, cancellationToken);
        if (tourist == null)
            return TouristErrors.NotFound(userContext.Id);

        tourist.SetPreferredLanguage(command.PreferredLanguage);
        var result = tourist.UpdateProfile(command.FirstName, command.LastName, command.Nationality);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
