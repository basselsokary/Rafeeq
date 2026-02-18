using Application.Common.Interfaces.Messaging.Requests.Base;

namespace Application.Common.Interfaces.Messaging.Requests;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
    