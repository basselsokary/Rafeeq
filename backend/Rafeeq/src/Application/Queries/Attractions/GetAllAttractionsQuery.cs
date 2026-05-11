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
    IAttractionQueryService queryService) : IQueryHandler<GetAllAttractionsQuery, PagedResult<AttractionListDto>>
{
    public async Task<Result<PagedResult<AttractionListDto>>> HandleAsync(GetAllAttractionsQuery query, CancellationToken cancellationToken)
    {
        var attractions = await queryService.GetAllAsync(query.SearchTerm, query.Type, query.Paging, cancellationToken);
        
        return Result.Success(attractions);
    }
}