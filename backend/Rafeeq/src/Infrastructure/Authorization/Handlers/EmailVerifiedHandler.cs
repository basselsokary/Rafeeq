using Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization.Handlers;

public class EmailVerifiedHandler : AuthorizationHandler<EmailVerifiedRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EmailVerifiedRequirement requirement)
    {
        var emailVerifiedClaim = context.User.FindFirst("email_verified")?.Value;

        if (emailVerifiedClaim == "true" ||
            context.User.HasClaim(c => c.Type == "email_verified" && c.Value == "true"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
