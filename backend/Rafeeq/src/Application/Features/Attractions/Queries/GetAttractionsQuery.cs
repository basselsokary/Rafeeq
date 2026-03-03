using Domain.Enums;
using FluentValidation;

namespace Application.Features.Attractions.Queries;

public record GetAttractionsQuery(AttractionType Type, string Search) : IQuery<GetAttractionsResponse>;

internal class GetAttractionsQueryHandler : IQueryHandler<GetAttractionsQuery, GetAttractionsResponse>
{
    public Task<Result<GetAttractionsResponse>> HandleAsync(GetAttractionsQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetAttractionsQueryValidator : AbstractValidator<GetAttractionsQuery>
{
    public GetAttractionsQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetAttractionsResponse(
    Guid Id,
    string Name,
    string Description,
    AttractionType Type,
    double Latitude,
    double Longitude,
    HistoricalPeriod HistoricalPeriod,
    string LocationDescription,
    IReadOnlyCollection<ImageDto> Images
);