using Application.Common.Interfaces.Messaging.Requests.Base;

namespace Application.Common.Interfaces.Messaging.Requests;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;