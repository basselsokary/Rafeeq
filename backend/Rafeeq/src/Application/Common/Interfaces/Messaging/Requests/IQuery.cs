using Application.Common.Interfaces.Messaging.Requests.Base;

namespace Application.Common.Interfaces.Messaging.Requests;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;