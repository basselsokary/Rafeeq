using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;

namespace Application.Commands.Users;

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

        var userResult = Tourist.Create(
            userId,
            command.FirstName,
            command.LastName,
            command.Email,
            command.Nationality);

        if (userResult.Failed)
            return userResult;
        
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
                command.Password);

            if (result.Failed)
                return result;

            await unitOfWork.Tourists.AddAsync(userResult.Value, cancellationToken);

            return Result.Success();

        }, cancellationToken);
    }
}
