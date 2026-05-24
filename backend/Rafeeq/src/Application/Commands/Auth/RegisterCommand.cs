using Application.Common.Interfaces.Authentication;
using Domain.Common.Constants;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;

namespace Application.Commands.Auth;

public sealed record RegisterCommand(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Nationality) : ICommand;

public sealed class RegisterCommandHandler(
    IIdentityService identityService,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterCommand>
{
    public async Task<Result> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        Guid userId = Guid.NewGuid();

        var touristResult = Tourist.Create(
            userId,
            command.FirstName,
            command.LastName,
            command.Nationality);

        if (touristResult.Failed)
            return touristResult;
        
        if (await identityService.IsUserExist(command.Email, cancellationToken))
            // Return success to avoid exposing whether the email is already taken or not,
            // which can be a security risk.
            return Result.Success();

        return await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var result = await identityService.RegisterAsync(
                userId,
                command.UserName,
                command.Email,
                UserRoles.Tourist,
                command.Password);

            if (result.Failed)
                return result;

            var tourist = touristResult.Value;
            await unitOfWork.Tourists.AddAsync(tourist, cancellationToken);
            tourist.RaiseDomainEvent(new TouristRegisteredEvent(command.Email, tourist.FirstName, result.Value));

            return Result.Success();

        }, cancellationToken);
    }
}
