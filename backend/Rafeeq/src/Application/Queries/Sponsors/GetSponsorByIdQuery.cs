using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sponsors;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors;

public sealed record GetSponsorByIdQuery(Guid Id) : IQuery<SponsorDetailDto>;

internal sealed class GetSponsorByIdQueryHandler(
    ISponsorQueryService queryService,
    IEnumLocalizer enumLocalizer,
    IUserContext userContext) : IQueryHandler<GetSponsorByIdQuery, SponsorDetailDto>
{
    public async Task<Result<SponsorDetailDto>> HandleAsync(GetSponsorByIdQuery query, CancellationToken cancellationToken)
    {
        var siteDetailDto = await queryService.GetByIdAsync(
            query.Id,
            userContext.Language,
            cancellationToken);

        if (siteDetailDto == null)
            return SponsorErrors.NotFound(query.Id);

        return Result.Success(siteDetailDto with
        {
            TypeDisplay = enumLocalizer.Localize(siteDetailDto.Type)
        });
    }
}
