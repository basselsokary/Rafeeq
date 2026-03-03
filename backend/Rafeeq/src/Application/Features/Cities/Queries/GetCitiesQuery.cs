using FluentValidation;

namespace Application.Features.Cities.Queries;

public record GetCitiesQuery(
) : IQuery<GetCitiesResponse>;

internal class GetCitiesQueryHandler : IQueryHandler<GetCitiesQuery, GetCitiesResponse>
{
    public Task<Result<GetCitiesResponse>> HandleAsync(GetCitiesQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetCitiesQueryValidator : AbstractValidator<GetCitiesQuery>
{
    public GetCitiesQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetCitiesResponse();