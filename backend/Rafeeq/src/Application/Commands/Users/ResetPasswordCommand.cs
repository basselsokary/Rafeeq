using Application.Common.Interfaces.Authentication;

namespace Application.Commands.Users;

public record ResetPasswordCommand(string Token, string NewPassword) : ICommand;

public class ResetPasswordCommandHandler(
    IIdentityService identityService,
    IUserContext userContext) : ICommandHandler<ResetPasswordCommand>
{
    public async Task<Result> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.ResetPasswordAsync(
            userContext.Id.ToString(),
            command.Token,
            command.NewPassword);

        if (result.Failed)
            return result;

        return Result.Success();
    }
}