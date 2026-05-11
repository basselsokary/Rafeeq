using Application.DTOs.Trips;

namespace Application.Common.Interfaces.Services;

public interface ITripPlannerService
{
    Task<Result<TripPlanResponse>> PlanTripAsync(
        TripPlanRequest request, CancellationToken ct = default);
}
