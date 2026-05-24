using Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization.Handlers;

public class RolesHandler : AuthorizationHandler<RolesRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RolesRequirement requirement)
    {
        var hasRole = requirement.Roles.Any(role => context.User.IsInRole(role));

        if (hasRole)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
