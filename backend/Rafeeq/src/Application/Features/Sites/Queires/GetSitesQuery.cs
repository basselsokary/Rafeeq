using FluentValidation;

namespace Application.Features.Sites.Queires;

public record GetSitesQuery(
) : IQuery<GetSitesResponse>;

internal class GetSitesQueryHandler : IQueryHandler<GetSitesQuery, GetSitesResponse>
{
    public Task<Result<GetSitesResponse>> HandleAsync(GetSitesQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetSitesQueryValidator : AbstractValidator<GetSitesQuery>
{
    public GetSitesQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetSitesResponse();