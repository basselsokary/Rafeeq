using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;

namespace Application.Queries.Sponsors.LocalizedContents;

public sealed record GetSponsorLocalizedContentsQuery(Guid SponsorId)
    : IQuery<List<AdminSponsorLocalizedContentDto>>;

internal sealed class GetSponsorLocalizedContentsQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSponsorLocalizedContentsQuery, List<AdminSponsorLocalizedContentDto>>
{
    public async Task<Result<List<AdminSponsorLocalizedContentDto>>> HandleAsync(GetSponsorLocalizedContentsQuery query, CancellationToken cancellationToken)
    {
        var localizedContentDtos = await queryService.GetLocalizedContentsAsync(query.SponsorId, cancellationToken);
    
        return localizedContentDtos;
    }
}
