using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors.LocalizedContents;

public sealed record GetSponsorLocalizedContentByIdQuery(Guid SponsorId, Guid ContentId) : IQuery<AdminSponsorLocalizedContentDto>;

internal sealed class GetSponsorLocalizedContentByIdQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetSponsorLocalizedContentByIdQuery, AdminSponsorLocalizedContentDto>
{
    public async Task<Result<AdminSponsorLocalizedContentDto>> HandleAsync(GetSponsorLocalizedContentByIdQuery query, CancellationToken cancellationToken)
    {
        var localizedContentDto = await queryService.GetLocalizedContentByIdAsync(query.SponsorId, query.ContentId, cancellationToken);
        if (localizedContentDto == null)
            return SponsorErrors.LocalizedContentNotFound;
    
        return Result.Success(localizedContentDto);
    }
}
