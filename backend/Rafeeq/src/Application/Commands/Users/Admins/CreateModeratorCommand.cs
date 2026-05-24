using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Services;
using Application.DTOs.Users;

namespace Application.Commands.Users.Admins;

public record CreateModeratorCommand(
    string FirstName,
    string LastName,
    string FullName,
    string Email) : ICommand<UserDetailsDto>;

internal sealed class CreateModeratorCommandHandler(
    IUserManagementService userManagementService,
    IUserCredentialService userCredentialService,
    IEmailService emailService) : ICommandHandler<CreateModeratorCommand, UserDetailsDto>
{
    public async Task<Result<UserDetailsDto>> HandleAsync(CreateModeratorCommand command, CancellationToken cancellationToken)
    {
        var userName = await userCredentialService.GenerateUniqueUsernameAsync(command.FirstName, command.LastName, cancellationToken);
        var password = userCredentialService.GenerateTemporaryPassword();

        Result<UserDetailsDto> result = await userManagementService.CreateModeratorAsync(
            userName,
            command.FirstName,
            command.LastName,
            command.FullName,
            command.Email,
            password,
            cancellationToken);
        
        if (result.Failed)
            return Result.Failure<UserDetailsDto>(result.Error);
        
        await emailService.SendWelcomeModeratorAsync(command.FirstName, command.Email, password, cancellationToken);
        
        return result;
    }
}
