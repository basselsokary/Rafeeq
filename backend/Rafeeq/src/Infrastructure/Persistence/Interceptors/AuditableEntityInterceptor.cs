using Domain.Common;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Interceptors;

internal sealed class AuditableEntityInterceptor(
    CurrentUserService currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null)
        {
            // logger.LogWarning("DbContext is null in AuditableEntityInterceptor. Skipping audit property updates.");
            return;
        }

        // logger.LogInformation("Updating auditable entities for user {UserId} ({UserName})", currentUser.UserId, currentUser.UserName);

        var entries = context.ChangeTracker.Entries<BaseAuditableEntity>();

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            // logger.LogInformation(
            //     "Updating auditable entity with ID {Id} and state {State}",
            //     entry.Entity.Id, entry.State);

            if (entry.State == EntityState.Added)
            {
                entry.Entity.SetCreated(now, currentUser.UserId, currentUser.UserName);
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.SetModified(now, currentUser.UserId, currentUser.UserName);
            }
        }
    }
}
