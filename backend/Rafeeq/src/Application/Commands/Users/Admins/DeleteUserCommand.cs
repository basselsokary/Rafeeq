using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public sealed record DeleteUserCommand(string Email) : ICommand;

internal sealed class DeleteUserCommandHandler(
    IAdminService adminService) : ICommandHandler<DeleteUserCommand>
{
    public async Task<Result> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        var result = await adminService.DeleteUserAsync(command.Email);
        if (result.Failed)
            return result;

        return Result.Success();
    }
}