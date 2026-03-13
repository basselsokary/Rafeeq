namespace Application.Commands.Users;

public record RefreshCommand(string RefreshToken) : ICommand<RefreshResponse>;

public class RefreshCommandHandler() : ICommandHandler<RefreshCommand, RefreshResponse>
{
    public async Task<Result<RefreshResponse>> HandleAsync(RefreshCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public record RefreshResponse(string Token, string RefreshToken);
