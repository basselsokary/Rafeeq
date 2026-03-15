using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;
using Domain.Enums;

namespace Application.Commands.Users;

public record RegisterCommand(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Nationality,
    LanguageCode PreferredLanguage) : ICommand;

public class RegisterCommandHandler(
    IIdentityService identityService,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterCommand>
{
    public async Task<Result> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.RegisterAsync(
            command.UserName,
            command.Email,
            command.Password);
        
        if (result.Failed)
            return result;

        Result<Tourist> userResult = Tourist.Create(
            result.Value,
            command.FirstName,
            command.LastName,
            command.Nationality,
            preferredLanguage: command.PreferredLanguage);

        if (userResult.Failed)
            return userResult;

        await unitOfWork.Tourists.AddAsync(userResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return result;
    }
}
