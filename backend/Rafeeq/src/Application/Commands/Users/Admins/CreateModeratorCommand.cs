using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public sealed record CreateModeratorCommand(
    string FirstName,
    string LastName,
    string FullName) : ICommand;

internal sealed class CreateModeratorCommandHandler(
    IAdminService adminService,
    IEmailGeneratorService emailGenerator,
    IPasswordGenerator passwordGenerator,
    IEmailService emailService) : ICommandHandler<CreateModeratorCommand>
{
    public async Task<Result> HandleAsync(CreateModeratorCommand command, CancellationToken cancellationToken)
    {
        var userName = await emailGenerator.GenerateUniqueUsernameAsync(command.FirstName, command.LastName, cancellationToken);
        var email = await emailGenerator.GenerateModeratorEmailAsync(command.FirstName, command.LastName, cancellationToken);
        var password = passwordGenerator.GenerateTemporaryPassword();

        Result result = await adminService.AddModeratorAsync(
            userName,
            command.FirstName,
            command.LastName,
            command.FullName,
            email,
            password,
            cancellationToken);
        
        if (result.Failed)
            return result;
        
        await emailService.SendWelcomeModeratorAsync(command.FirstName, email, password, cancellationToken);
        
        return Result.Success();
    }
}
