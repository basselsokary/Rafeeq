using FluentValidation;

namespace Application.Features.Cities.Queries;

public record GetCityByIdQuery(
) : IQuery<GetCityByIdResponse>;

internal class GetCityByIdQueryHandler : IQueryHandler<GetCityByIdQuery, GetCityByIdResponse>
{
    public Task<Result<GetCityByIdResponse>> HandleAsync(GetCityByIdQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetCityByIdQueryValidator : AbstractValidator<GetCityByIdQuery>
{
    public GetCityByIdQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetCityByIdResponse();