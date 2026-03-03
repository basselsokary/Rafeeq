using FluentValidation;

namespace Application.Features.Users.Commands;

public record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;

public class LoginCommandHandler() : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        throw new NotImplementedException();
    }
}

public record LoginResponse(string Token, string RefreshToken);