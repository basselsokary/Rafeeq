using FluentValidation;

namespace Application.Features.Sites.Queires;

public record GetSiteByIdQuery(
) : IQuery<GetSiteByIdResponse>;

internal class GetSiteByIdQueryHandler : IQueryHandler<GetSiteByIdQuery, GetSiteByIdResponse>
{
    public Task<Result<GetSiteByIdResponse>> HandleAsync(GetSiteByIdQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetSiteByIdQueryValidator : AbstractValidator<GetSiteByIdQuery>
{
    public GetSiteByIdQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetSiteByIdResponse();