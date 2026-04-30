using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Authentication;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Queries.Attractions;

public sealed record GetAttractionsBySiteIdQuery(
    Guid SiteId,
    AttractionType? Type,
    PagingParameters Paging) : IQuery<PagedResult<AttractionListDto>>;

internal sealed class GetAttractionsBySiteIdQueryHandler(
    IAttractionQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetAttractionsBySiteIdQuery, PagedResult<AttractionListDto>>
{
    public async Task<Result<PagedResult<AttractionListDto>>> HandleAsync(GetAttractionsBySiteIdQuery query, CancellationToken cancellationToken)
    {
        PagedResult<AttractionListDto> pagedResult = await queryService.GetBySiteIdAsync(
            query.SiteId,
            query.Type,
            query.Paging,
            userContext.Language,
            cancellationToken);

        var localizedData = pagedResult.Data.Select(attraction => attraction with
        {
            TypeDisplay = enumLocalizer.Localize(attraction.Type)
        }).ToList();

        return Result.Success(pagedResult with { Data = localizedData });
    }
}