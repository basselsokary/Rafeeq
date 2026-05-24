using API.Controllers.Base;
using Application.Commands.Trips;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.Trips;
using Application.Queries.Trips;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/trips")]
[Authorize(Policy = Policies.TouristOnly)]
public class TripsController : ApiBaseController
{
    #region CRUD Operations
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TripDetailDto>> GetById(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetTripByIdQuery, TripDetailDto> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await queryHandler.HandleAsync(new GetTripByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TripListDto>>> GetMyTrips(
        [FromServices] IQueryHandler<GetTouristTripsQuery, PagedResult<TripListDto>> queryHandler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var paging = new PagingParameters(page, pageSize);
        var result = await queryHandler.HandleAsync(new GetTouristTripsQuery(paging), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateTripCommand command,
        [FromServices] ICommandHandler<CreateTripCommand, Guid> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        
        return HandleResult(result);

        // return CreatedResult(nameof(GetById), new { id = result.Value }, result.Value);
    }

    public sealed record UpdateTripRequest(double Latitude, double Longitude, string Name, string? Description, DateTime StartDate, DateTime EndDate, decimal? EstimatedBudget, string Currency);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateTripRequest request,
        [FromServices] ICommandHandler<UpdateTripCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateTripCommand(
            id,
            request.Latitude,
            request.Longitude,
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.EstimatedBudget,
            request.Currency);

        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<DeleteTripCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(new DeleteTripCommand(id), cancellationToken);
        return HandleResult(result);
    }
    #endregion
}
