using Application.Common.Interfaces.Messaging.Behavior;
using Application.Common.Interfaces.Messaging.Requests.Base;

namespace API.Services.Dispatchers;

public class RequestDispatcher : IRequestDispatcher
{
    private static readonly string _handleName = "HandleAsync";
    private readonly IServiceProvider _serviceProvider;

    public RequestDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> DispatchAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);

        var handleDelegate = async () => await (Task<TResponse>)handler
                        .GetType()
                        .GetMethod(_handleName)!
                        .Invoke(handler, [request, cancellationToken])!;

        return await HandleBehaviors(request, handleDelegate, cancellationToken);
    }

    private async Task<TResponse> HandleBehaviors<TResponse>(IRequest<TResponse> request, Func<Task<TResponse>> handleDelegate, CancellationToken cancellationToken)
    {
        var behaviorTypes = typeof(IPipelineBehavior<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var behaviors = _serviceProvider
            .GetServices(behaviorTypes)
            .Cast<object>()
            .Reverse()
            .ToList();

        // Chain behaviors like a middleware pipeline
        Func<Task<TResponse>> pipeline = handleDelegate;

        foreach (var behavior in behaviors)
        {
            var b = behavior; // prevent modified closure issue
            var method = b.GetType().GetMethod(_handleName);
            var next = pipeline;

            pipeline = () => (Task<TResponse>)method?.Invoke(b, [request, next, cancellationToken])!;
        }

        return await pipeline();
    }
}
