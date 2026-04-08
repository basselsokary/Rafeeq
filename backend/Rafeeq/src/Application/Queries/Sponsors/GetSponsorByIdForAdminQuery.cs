using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sponsors;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors;

public record GetSponsorByIdForAdminQuery(Guid Id) : IQuery<SponsorAdminDetailDto>;

internal class GetSponsorByIdForAdminQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSponsorByIdForAdminQuery, SponsorAdminDetailDto>
{
    public async Task<Result<SponsorAdminDetailDto>> HandleAsync(GetSponsorByIdForAdminQuery query, CancellationToken cancellationToken)
    {
        var siteDetailDto = await queryService.GetByIdForAdminAsync(query.Id, cancellationToken);
        if (siteDetailDto == null)
            return SponsorErrors.NotFound(query.Id);

        return Result.Success(siteDetailDto);
    }
}
