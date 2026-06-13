using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sites;

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
        var mustVisitTask = siteQueryService
            .GetMustVisitAsync(
                language: userContext.Language,
                cancellationToken: cancellationToken);

        var hiddenGemsTask = siteQueryService
            .GetHiddenGemsAsync(
                language: userContext.Language,
                cancellationToken: cancellationToken);

        var featuredDealsTask = sponsorQueryService
            .GetActiveOffersAsync(
                language: userContext.Language,
                cancellationToken: cancellationToken);

        Task<List<SiteSummaryDto>>? nearbyTask = null;

        // Near You (if location provided)
        if (query.Latitude.HasValue && query.Longitude.HasValue)
        {
            nearbyTask = siteQueryService
                .GetNearbyAsync(
                    query.Latitude.Value,
                    query.Longitude.Value,
                    language: userContext.Language,
                    cancellationToken: cancellationToken);
        }

        await Task.WhenAll(
            mustVisitTask,
            hiddenGemsTask,
            featuredDealsTask,
            nearbyTask ?? Task.FromResult<List<SiteSummaryDto>>([]));

        var mustVisitSites = (await mustVisitTask)
            .Select(s => s with { TypeDisplay = enumLocalizer.Localize(s.Type) })
            .ToList();

        var hiddenGems = (await hiddenGemsTask)
            .Select(s => s with { TypeDisplay = enumLocalizer.Localize(s.Type) })
            .ToList();

        var featuredDeals = await featuredDealsTask;

        var nearbySites = (nearbyTask is null)
            ? []
            : (await nearbyTask)
                .Select(s => s with { TypeDisplay = enumLocalizer.Localize(s.Type) })
                .ToList();

        return new HomeScreenDto(
            MustVisit: mustVisitSites,
            HiddenGems: hiddenGems,
            NearYou: nearbySites,
            FeaturedDeals: featuredDeals
        );
    }
}
