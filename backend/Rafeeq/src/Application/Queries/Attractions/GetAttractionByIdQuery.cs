using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Authentication;
using Application.DTOs.Attractions;
using Domain.Entities.AttractionAggregate;

namespace Application.Queries.Attractions;

public sealed record GetAttractionByIdQuery(Guid Id) : IQuery<AttractionDetailDto>;

internal sealed class GetAttractionByIdQueryHandler(
    IAttractionQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetAttractionByIdQuery, AttractionDetailDto>
{
    public async Task<Result<AttractionDetailDto>> HandleAsync(GetAttractionByIdQuery query, CancellationToken cancellationToken)
    {
        var attractionDto = await queryService.GetByIdAsync(
            query.Id,
            userContext.Language,
            cancellationToken);
        if (attractionDto == null)
            return AttractionErrors.NotFound(query.Id);

        return Result.Success(
            attractionDto with
            {
                TypeDisplay = enumLocalizer.Localize(attractionDto.Type),
                
                // HistoricalPeriodDisplay = attractionDto.HistoricalPeriods
                //     .Select(hp => enumLocalizer.Localize(hp))
                //     .ToList(),
            });

    }
}