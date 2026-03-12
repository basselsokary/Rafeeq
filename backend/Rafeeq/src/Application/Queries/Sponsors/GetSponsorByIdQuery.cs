using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sponsors;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors;

public record GetSponsorByIdQuery(Guid Id) : IQuery<SponsorDetailDto>;

internal class GetSponsorByIdQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSponsorByIdQuery, SponsorDetailDto>
{
    public async Task<Result<SponsorDetailDto>> HandleAsync(GetSponsorByIdQuery query, CancellationToken cancellationToken)
    {
        var siteDetailDto = await queryService.GetByIdAsync(query.Id, cancellationToken);
        if (siteDetailDto == null)
            return SponsorErrors.NotFound(query.Id);

        return Result.Success(siteDetailDto);
    }
}
