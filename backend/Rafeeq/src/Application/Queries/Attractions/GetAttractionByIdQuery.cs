using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Attractions;
using Domain.Entities.AttractionAggregate;

namespace Application.Queries.Attractions;

public record GetAttractionByIdQuery(Guid Id) : IQuery<AttractionDetailDto>;

internal class GetAttractionByIdQueryHandler(
    IAttractionQueryService queryService) : IQueryHandler<GetAttractionByIdQuery, AttractionDetailDto>
{
    public async Task<Result<AttractionDetailDto>> HandleAsync(GetAttractionByIdQuery query, CancellationToken cancellationToken)
    {
        var attractionDto = await queryService.GetByIdAsync(query.Id, cancellationToken);
        if (attractionDto == null)
            return AttractionErrors.NotFound(query.Id);

        return Result.Success(attractionDto);

    }
}