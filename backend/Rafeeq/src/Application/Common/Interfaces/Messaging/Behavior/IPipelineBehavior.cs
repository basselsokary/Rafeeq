using Application.Common.Interfaces.Messaging.Requests.Base;

namespace Application.Common.Interfaces.Messaging.Behavior;

public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken);
}

