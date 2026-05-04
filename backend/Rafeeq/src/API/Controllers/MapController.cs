using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.Queries.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class MapController : ApiBaseController
{
    [HttpGet("places")]
    public async Task<ActionResult<List<MapPlaceMarkerDto>>> GetMapPlaces(
        [FromServices] IQueryHandler<GetMapPlacesQuery, List<MapPlaceMarkerDto>> handler,
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] int radiusKm = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMapPlacesQuery(latitude, longitude, radiusKm);
        var result = await handler.HandleAsync(query, cancellationToken);
        
        return HandleResult(result);
    }
    
}
