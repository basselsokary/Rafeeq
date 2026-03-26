namespace Application.Common.Interfaces.Services;

public interface IModeratorService
{
    Task<Result> ActivateUserAsync(Guid userId, bool activate);
}
