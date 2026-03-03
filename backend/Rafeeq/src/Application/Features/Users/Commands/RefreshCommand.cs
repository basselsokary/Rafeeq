using FluentValidation;

namespace Application.Features.Users.Commands;

public record RefreshCommand(string RefreshToken) : ICommand<RefreshResponse>;

public class RefreshCommandHandler() : ICommandHandler<RefreshCommand, RefreshResponse>
{
    public async Task<Result<RefreshResponse>> HandleAsync(RefreshCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        throw new NotImplementedException();
    }
}

public record RefreshResponse(string Token, string RefreshToken);
