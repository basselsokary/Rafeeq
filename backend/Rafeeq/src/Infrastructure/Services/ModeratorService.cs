using Application.Common.Interfaces.Services;
using Infrastructure.Identity.Entities;
using Infrastructure.Persistence.ApplicationContext;
using Shared;

namespace Infrastructure.Services;

internal class ModeratorService(
    ApplicationDbContext context) : IModeratorService
{
    public async Task<Result> ActivateTouristAsync(Guid userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return ApplicationUserErrors.NotFound(userId);

        user.Activate();

        context.SaveChanges();

        return Result.Success();
    }
}
