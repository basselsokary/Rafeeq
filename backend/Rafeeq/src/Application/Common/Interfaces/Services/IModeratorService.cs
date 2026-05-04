namespace Application.Common.Interfaces.Services;

public interface IModeratorService
{
    Task<Result> ActivateTouristAsync(Guid touristId);
}