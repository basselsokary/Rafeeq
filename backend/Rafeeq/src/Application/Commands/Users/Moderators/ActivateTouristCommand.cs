using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Moderators;

public sealed record ActivateTouristCommand(
    Guid TouristId) : ICommand;

public sealed class ActivateTouristCommandHandler(
    IModeratorService moderatorService) : ICommandHandler<ActivateTouristCommand>
{
    public async Task<Result> HandleAsync(ActivateTouristCommand command, CancellationToken cancellationToken)
    {
        var result = await moderatorService.ActivateTouristAsync(
            command.TouristId);

        if (result.Failed)
            return result;

        return Result.Success();
    }
}