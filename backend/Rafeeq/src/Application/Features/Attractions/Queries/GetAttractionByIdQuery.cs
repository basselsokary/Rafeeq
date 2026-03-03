using Domain.Enums;
using FluentValidation;

namespace Application.Features.Attractions.Queries;

public record GetAttractionByIdQuery(Guid Id) : IQuery<GetAttractionByIdResponse>;

internal class GetAttractionByIdQueryHandler : IQueryHandler<GetAttractionByIdQuery, GetAttractionByIdResponse>
{
    public Task<Result<GetAttractionByIdResponse>> HandleAsync(GetAttractionByIdQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetAttractionByIdQueryValidator : AbstractValidator<GetAttractionByIdQuery>
{
    public GetAttractionByIdQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetAttractionByIdResponse(
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