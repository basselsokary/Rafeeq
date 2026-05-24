using Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization.Handlers;

public class AccountActiveHandler : AuthorizationHandler<AccountActiveRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AccountActiveRequirement requirement)
    {
        var status = context.User.FindFirst("account_status")?.Value;

        // If no claim present, assume active (backward compat)
        if (string.IsNullOrEmpty(status) || status == "Active")
            context.Succeed(requirement);

        // Locked/Suspended/Deleted → do NOT succeed → 403 returned

        return Task.CompletedTask;
    }
}
