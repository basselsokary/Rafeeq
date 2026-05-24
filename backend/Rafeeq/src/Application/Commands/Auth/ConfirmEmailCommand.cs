using Application.Common.Interfaces.Authentication;

namespace Application.Commands.Auth;

public sealed record ConfirmEmailCommand(string Token, string Email) : ICommand;

public sealed class ConfirmEmailCommandHandler(
    IIdentityService identityService) : ICommandHandler<ConfirmEmailCommand>
{
    public async Task<Result> HandleAsync(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.ConfirmEmailAsync(
            command.Token,
            command.Email);

        if (result.Failed)
            return result;

        return Result.Success();
    }
}