using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Attractions;
using Domain.Entities.AttractionAggregate;

namespace Application.Queries.Attractions;

public record GetAttractionByIdForAdminQuery(Guid Id) : IQuery<AttractionAdminDetailDto>;

internal class GetAttractionByIdForAdminQueryHandler(
    IAttractionQueryService queryService) : IQueryHandler<GetAttractionByIdForAdminQuery, AttractionAdminDetailDto>
{
    public async Task<Result<AttractionAdminDetailDto>> HandleAsync(GetAttractionByIdForAdminQuery query, CancellationToken cancellationToken)
    {
        var attractionDto = await queryService.GetByIdForAdminAsync(query.Id, cancellationToken);
        if (attractionDto == null)
            return AttractionErrors.NotFound(query.Id);

        return Result.Success(attractionDto);

    }
}