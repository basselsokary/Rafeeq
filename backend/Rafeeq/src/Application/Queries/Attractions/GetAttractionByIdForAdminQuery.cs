using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Authentication;
using Application.DTOs.Attractions;
using Domain.Entities.AttractionAggregate;

namespace Application.Queries.Attractions;

public sealed record GetAttractionByIdForAdminQuery(Guid Id) : IQuery<AttractionAdminDetailDto>;

internal sealed class GetAttractionByIdForAdminQueryHandler(
    IAttractionQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetAttractionByIdForAdminQuery, AttractionAdminDetailDto>
{
    public async Task<Result<AttractionAdminDetailDto>> HandleAsync(GetAttractionByIdForAdminQuery query, CancellationToken cancellationToken)
    {
        var attractionDto = await queryService.GetByIdForAdminAsync(
            query.Id,
            userContext.Language,
            cancellationToken);
        if (attractionDto == null)
            return AttractionErrors.NotFound(query.Id);

        return Result.Success(attractionDto);

    }
}