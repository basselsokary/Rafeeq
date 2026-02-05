using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Messaging.Behavior;
using Application.Common.Interfaces.Messaging.Requests.Base;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;
    // private readonly IUserContext _user;
    // private readonly IIdentityService _identityService;

    public LoggingBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = /*_user.Id*/ null ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            // userName = await _identityService.GetUserNameAsync(userId);
            userName = "UserName";
        }

        _logger.LogInformation("Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);

        return await next();
    }
}
