using Application.DTOs.Sites;
using Application.DTOs.Sponsors;

namespace Application.DTOs.Common;

public record HomeScreenDto(
    List<SiteSummaryDto> MustVisit,
    List<SiteSummaryDto> HiddenGems,
    List<SiteSummaryDto> NearYou,
    List<SponsorOfferSummaryDto> FeaturedDeals);
