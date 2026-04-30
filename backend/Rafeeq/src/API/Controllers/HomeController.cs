using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.Queries.Home;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class HomeController : ApiBaseController
{
    [HttpGet]
    public async Task<ActionResult<HomeScreenDto>> GetHomeScreen(
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromServices] IQueryHandler<GetHomeDataQuery, HomeScreenDto> _queryHandler,
        CancellationToken cancellationToken = default)
    {
        // PrintHeaderData();
        var query = new GetHomeDataQuery(latitude, longitude);
        var result = await _queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    private void PrintHeaderData()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        var headers = Request.Headers;
        Console.WriteLine("Received request with headers:");
        foreach (var header in headers)
        {
            Console.WriteLine($"{header.Key}: {header.Value}");
        }
        Console.ResetColor();
    }
}
