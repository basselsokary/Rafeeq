using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors;

public sealed record GetSponsorByIdForAdminQuery(Guid Id) : IQuery<AdminSponsorDetailDto>;

internal sealed class GetSponsorByIdForAdminQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSponsorByIdForAdminQuery, AdminSponsorDetailDto>
{
    public async Task<Result<AdminSponsorDetailDto>> HandleAsync(GetSponsorByIdForAdminQuery query, CancellationToken cancellationToken)
    {
        var siteDetailDto = await queryService.GetByIdForAdminAsync(query.Id, cancellationToken);
        if (siteDetailDto == null)
            return SponsorErrors.NotFound(query.Id);

        return Result.Success(siteDetailDto);
    }
}
