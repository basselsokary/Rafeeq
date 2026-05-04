using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Authentication;
using Application.DTOs.Cities;

namespace Application.Queries.Cities;

public sealed record GetCitySummariesQuery : IQuery<List<CitySummaryDto>>;

internal sealed class GetCitySummariesQueryHandler(
    ICityQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetCitySummariesQuery, List<CitySummaryDto>>
{
    public async Task<Result<List<CitySummaryDto>>> HandleAsync(GetCitySummariesQuery query, CancellationToken cancellationToken)
    {
        var citySummaryDtos = await queryService.GetSummariesAsync(
            userContext.Language,
            cancellationToken);

        return Result.Success(citySummaryDtos);
    }
}
