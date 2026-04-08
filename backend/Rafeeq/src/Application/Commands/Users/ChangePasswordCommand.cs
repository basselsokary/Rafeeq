using Application.Common.Interfaces.Authentication;

namespace Application.Commands.Users;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : ICommand;

public class ChangePasswordCommandHandler(
    IIdentityService identityService,
    IUserContext userContext) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.ChangePasswordAsync(
            userContext.Id,
            command.CurrentPassword,
            command.NewPassword);

        if (result.Failed)
            return result;

        return Result.Success();
    }
}
