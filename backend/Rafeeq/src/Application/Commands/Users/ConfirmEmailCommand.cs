using Application.Common.Interfaces.Authentication;

namespace Application.Commands.Users;

public record ConfirmEmailCommand(string Token) : ICommand;

public class ConfirmEmailCommandHandler(
    IIdentityService identityService,
    IUserContext userContext) : ICommandHandler<ConfirmEmailCommand>
{
    public async Task<Result> HandleAsync(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.ConfirmEmailAsync(
            userContext.Id,
            command.Token);

        if (result.Failed)
            return result;

        return Result.Success();
    }
}