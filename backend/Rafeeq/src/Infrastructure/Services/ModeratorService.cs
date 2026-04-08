using Application.Common.Interfaces.Services;
using Infrastructure.Identity;
using Infrastructure.Persistence.ApplicationContext;
using Shared.Models;

namespace Infrastructure.Services;

internal class ModeratorService(
    ApplicationDbContext context) : IModeratorService
{
    public async Task<Result> ActivateUserAsync(Guid userId, bool activate)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return ApplicationUserErrors.NotFound(userId);

        if (activate)
        {
            user.Activate();
        }
        else
        {
            user.Deactivate();
        }

        context.SaveChanges();

        return Result.Success();
    }
}
