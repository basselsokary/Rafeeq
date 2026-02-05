using Application.Common.Interfaces.Messaging.Requests.Base;

namespace API.Services.Dispatchers;

public interface IRequestDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}