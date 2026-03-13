using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Cities;

namespace Application.Queries.Cities;

public record GetCitySummariesQuery : IQuery<List<CitySummaryDto>>;

internal class GetCitySummariesQueryHandler(
    ICityQueryService queryService) : IQueryHandler<GetCitySummariesQuery, List<CitySummaryDto>>
{
    public async Task<Result<List<CitySummaryDto>>> HandleAsync(GetCitySummariesQuery query, CancellationToken cancellationToken)
    {
        var citySummaryDtos = await queryService.GetAsync(cancellationToken);

        return Result.Success(citySummaryDtos);
    }
}
