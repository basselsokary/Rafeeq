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
        Guid userId = Guid.NewGuid();
        var userResult = Tourist.Create(
            userId,
            command.FirstName,
            command.LastName,
            command.Nationality,
            command.PreferredLanguage);

        if (userResult.Failed)
            return userResult;

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var result = await identityService.RegisterAsync(
                userId,
                command.UserName,
                command.Email,
                command.Password);
            
            if (result.Failed)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                /// Note: We return success to avoid exposing whether the email 
                /// is already registered or not, which can be a security risk.
                return Result.Success();
            }

            await unitOfWork.Tourists.AddAsync(userResult.Value, cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(Error.Failure("", "An error occurred while registering the user."));
        }
        
        return Result.Success();
    }
}
