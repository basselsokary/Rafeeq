using System.Collections.Concurrent;
using System.Reflection;
using Application.Common.Interfaces.Messaging.Behavior;
using Application.Common.Interfaces.Messaging.Requests.Base;

namespace API.Services.Dispatchers;

/// <summary>
/// Custom request dispatcher implementing a MediatR-like pipeline pattern.
/// </summary>
public class RequestDispatcher : IRequestDispatcher
{
    private const string HandleMethodName = "HandleAsync";
    private static readonly ConcurrentDictionary<Type, MethodInfo> _handlerMethodCache = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> _behaviorMethodCache = new();
    
    private readonly IServiceProvider _serviceProvider;

    public RequestDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<TResponse> DispatchAsync<TResponse>(
        IRequest<TResponse> request, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        
        var handleMethod = GetCachedHandlerMethod(handlerType);
        var handleDelegate = CreateHandlerDelegate(request, handler, handleMethod, cancellationToken);
        
        return await HandleBehaviors(request, handleDelegate, cancellationToken);
    }

    private static MethodInfo GetCachedHandlerMethod(Type handlerType)
    {
        return _handlerMethodCache.GetOrAdd(handlerType, type => type.GetMethod(HandleMethodName) 
            ?? throw new InvalidOperationException(
                $"Handler method '{HandleMethodName}' not found on {type.Name}")
        );
    }

    private static Func<Task<TResponse>> CreateHandlerDelegate<TResponse>(
        IRequest<TResponse> request, 
        object handler, 
        MethodInfo method, 
        CancellationToken cancellationToken)
    {
        return async () =>
        {
            var result = method.Invoke(handler, [request, cancellationToken]);
            
            if (result is not Task<TResponse> task)
            {
                throw new InvalidOperationException(
                    $"Handler method returned {result?.GetType().Name ?? "null"} instead of Task<{typeof(TResponse).Name}>");
            }
            return await task;
        };
    }

    private async Task<TResponse> HandleBehaviors<TResponse>(
        IRequest<TResponse> request, 
        Func<Task<TResponse>> handleDelegate, 
        CancellationToken cancellationToken)
    {
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var behaviors = _serviceProvider
            .GetServices(behaviorType)
            .Cast<object>()
            .Reverse()
            .ToList();

        Func<Task<TResponse>> pipeline = handleDelegate;
        
        foreach (var behavior in behaviors)
        {
            var currentBehavior = behavior; // prevent modified closure issue
            var method = GetCachedBehaviorMethod(currentBehavior.GetType());
            var currentPipeline = pipeline;
            
            pipeline = CreateBehaviorDelegate(currentBehavior, method, request, currentPipeline, cancellationToken);
        }

        return await pipeline();
    }

    private static MethodInfo GetCachedBehaviorMethod(Type behaviorType)
    {
        return _behaviorMethodCache.GetOrAdd(behaviorType, type =>
            type.GetMethod(HandleMethodName) 
            ?? throw new InvalidOperationException($"Behavior method '{HandleMethodName}' not found on {type.Name}")
        );
    }

    private static Func<Task<TResponse>> CreateBehaviorDelegate<TResponse>(
        object behavior,
        MethodInfo method,
        IRequest<TResponse> request,
        Func<Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        return () =>
        {
            var result = method.Invoke(behavior, [request, next, cancellationToken]);
            if (result is not Task<TResponse> task)
            {
                throw new InvalidOperationException(
                    $"Behavior method returned {result?.GetType().Name ?? "null"} instead of Task<{typeof(TResponse).Name}>");
            }
            return task;
        };
    }
}