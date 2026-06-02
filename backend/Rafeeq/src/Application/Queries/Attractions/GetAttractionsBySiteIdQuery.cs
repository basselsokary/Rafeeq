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
    string? SearchTerm = null) : IQuery<List<AttractionListDto>>;

internal sealed class GetAttractionsBySiteIdQueryHandler(
    IAttractionQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetAttractionsBySiteIdQuery, List<AttractionListDto>>
{
    public async Task<Result<List<AttractionListDto>>> HandleAsync(GetAttractionsBySiteIdQuery query, CancellationToken cancellationToken)
    {
        List<AttractionListDto> attractions = await queryService.GetBySiteIdAsync(
            query.SiteId,
            query.Type,
            query.SearchTerm,
            userContext.Language,
            cancellationToken);

        var localizedAttractions = attractions.Select(attraction => attraction with
        {
            TypeDisplay = enumLocalizer.Localize(attraction.Type)
        }).ToList();

        return Result.Success(localizedAttractions);
    }
}