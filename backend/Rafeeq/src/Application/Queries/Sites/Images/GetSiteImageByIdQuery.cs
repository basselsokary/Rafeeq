using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites.Images;

public sealed record GetSiteImageByIdQuery(Guid SiteId, Guid ImageId) : IQuery<ImageDto>;

internal sealed class GetSiteImageByIdQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteImageByIdQuery, ImageDto>
{
    public async Task<Result<ImageDto>> HandleAsync(GetSiteImageByIdQuery query, CancellationToken cancellationToken)
    {
        var image = await queryService.GetImageByIdAsync(query.SiteId, query.ImageId, cancellationToken);
        if (image is null)
            return SiteErrors.ImageNotFound;

        return Result.Success(image);
    }
}
