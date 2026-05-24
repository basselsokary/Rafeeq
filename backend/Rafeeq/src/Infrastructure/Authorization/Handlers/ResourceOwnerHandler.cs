using System.Security.Claims;
using Domain.Common.Constants;
using Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization.Handlers;

public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement, Guid>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnerRequirement requirement,
        Guid resourceOwnerId)
    {
        // Admin/Moderator override
        if (requirement.AllowAdminOverride &&
            (context.User.IsInRole(UserRoles.SuperAdmin)
                || context.User.IsInRole(UserRoles.Admin)
                || context.User.IsInRole(UserRoles.Moderator)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Check ownership
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId) && userId == resourceOwnerId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
