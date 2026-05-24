using Application.Common.Interfaces.Authentication;

namespace Application.Commands.Auth;

public sealed record ResetPasswordCommand(string Token, string Email, string NewPassword) : ICommand;

public sealed class ResetPasswordCommandHandler(
    IIdentityService identityService) : ICommandHandler<ResetPasswordCommand>
{
    public async Task<Result> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.ResetPasswordAsync(
            command.Token,
            command.Email,
            command.NewPassword);

        if (result.Failed)
            return result;

        return Result.Success();
    }
}