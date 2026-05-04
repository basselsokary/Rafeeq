using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sites;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites;

public sealed record GetSiteByIdQuery(Guid Id) : IQuery<SiteDetailDto>;

internal sealed class GetSiteByIdQueryHandler(
    ISiteQueryService queryService,
    IEnumLocalizer enumLocalizer,
    IUserContext userContext) : IQueryHandler<GetSiteByIdQuery, SiteDetailDto>
{
    public async Task<Result<SiteDetailDto>> HandleAsync(GetSiteByIdQuery query, CancellationToken cancellationToken)
    {
        var siteDetailDto = await queryService.GetByIdAsync(query.Id, userContext.Language, cancellationToken);
        if (siteDetailDto == null)
            return SiteErrors.NotFound(query.Id);

        return Result.Success(siteDetailDto with
        {
            FacilityTypeDisplays = siteDetailDto.FacilityTypes
                .Select(ft => enumLocalizer.Localize(ft))
                .ToList(),

            OpeningHours = siteDetailDto.OpeningHours
                .Select(oh => oh with
                {
                    DayDisplay = enumLocalizer.Localize(oh.Day)
                })
                .ToList(),
            
            NearestTransportations = siteDetailDto.NearestTransportations
                .Select(nt => nt with
                {
                    TypeDisplay = enumLocalizer.Localize(nt.Type)
                })
                .ToList(),
            
            TypeDisplay = enumLocalizer.Localize(siteDetailDto.Type),
            StatusDisplay = enumLocalizer.Localize(siteDetailDto.Status)
        });
    }
}