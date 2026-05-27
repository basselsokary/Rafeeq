using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Queries.Attractions;

public sealed record GetAllAttractionsQuery(
    string? SearchTerm,
    AttractionType? Type,
    PagingParameters Paging) : IQuery<PagedResult<AttractionListDto>>;

internal sealed class GetAllAttractionsQueryHandler(
    IAttractionQueryService queryService,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetAllAttractionsQuery, PagedResult<AttractionListDto>>
{
    public async Task<Result<PagedResult<AttractionListDto>>> HandleAsync(GetAllAttractionsQuery query, CancellationToken cancellationToken)
    {
        var attractions = await queryService.GetAllAsync(query.SearchTerm, query.Type, query.Paging, cancellationToken);

        var localizedData = attractions.Data.Select(attraction => attraction with
        {
            TypeDisplay = enumLocalizer.Localize(attraction.Type)
        }).ToList();

        return Result.Success(attractions with { Data = localizedData });
    }
}