using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sites;
using Application.DTOs.Sponsors;

namespace Application.Queries.Home;

public sealed record GetHomeDataQuery(
    double? Latitude,
    double? Longitude) : IQuery<HomeScreenDto>;

public sealed class GetHomeDataQueryHandler(
    ISiteQueryService siteQueryService,
    ISponsorQueryService sponsorQueryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetHomeDataQuery, HomeScreenDto>
{
    public async Task<Result<HomeScreenDto>> HandleAsync(GetHomeDataQuery query, CancellationToken cancellationToken)
    {
        List<SiteSummaryDto> mustVisitSites = [];
        List<SiteSummaryDto> hiddenGems = [];
        List<SiteSummaryDto> nearbySites = [];
        List<SponsorOfferSummaryDto> featuredDeals = [];

        mustVisitSites = await siteQueryService
            .GetMustVisitAsync(
                language: userContext.Language,
                cancellationToken: cancellationToken);
        
        mustVisitSites = mustVisitSites
            .Select(s => s with { TypeDisplay = enumLocalizer.Localize(s.Type) })
            .ToList();

        hiddenGems = await siteQueryService
            .GetHiddenGemsAsync(
                language: userContext.Language,
                cancellationToken: cancellationToken);

        hiddenGems = hiddenGems
            .Select(s => s with { TypeDisplay = enumLocalizer.Localize(s.Type) })
            .ToList();

        // result.QuickCategories = await cityQueryService
        //     .GetAsync(new PagingParameters(),cancellationToken);
            
        featuredDeals = await sponsorQueryService.GetActiveOffersAsync(cancellationToken: cancellationToken);            

        // Near You (if location provided)
        if (query.Latitude.HasValue && query.Longitude.HasValue)
        {
            nearbySites = await siteQueryService
                .GetNearbyAsync(
                    query.Latitude.Value,
                    query.Longitude.Value,
                    language: userContext.Language,
                    cancellationToken: cancellationToken);
                
            nearbySites = nearbySites
                .Select(s => s with { TypeDisplay = enumLocalizer.Localize(s.Type) })
                .ToList();
        }

        return new HomeScreenDto(
            MustVisit: mustVisitSites,
            HiddenGems: hiddenGems,
            NearYou: nearbySites,
            FeaturedDeals: featuredDeals
        );
    }
}
