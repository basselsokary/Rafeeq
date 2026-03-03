using FluentValidation;

namespace Application.Features.Sites.Queires;

public record GetNearbySitesQuery(
) : IQuery<GetNearbySitesResponse>;

internal class GetNearbySitesQueryHandler : IQueryHandler<GetNearbySitesQuery, GetNearbySitesResponse>
{
    public Task<Result<GetNearbySitesResponse>> HandleAsync(GetNearbySitesQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetNearbySitesQueryValidator : AbstractValidator<GetNearbySitesQuery>
{
    public GetNearbySitesQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetNearbySitesResponse();