using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Trips;
using Domain.Entities.TripAggregate;

namespace Application.Queries.Trips;

public sealed record GetTripByIdQuery(Guid Id) : IQuery<TripDetailDto>;

internal sealed class GetTripByIdQueryHandler(
    ITripQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetTripByIdQuery, TripDetailDto>
{
    public async Task<Result<TripDetailDto>> HandleAsync(GetTripByIdQuery query, CancellationToken cancellationToken)
    {
        var tripDto = await queryService.GetByIdAsync(query.Id, userContext.Id, cancellationToken);
        if (tripDto == null)
            return TripErrors.NotFound;

        return Result.Success(tripDto);
    }
}
