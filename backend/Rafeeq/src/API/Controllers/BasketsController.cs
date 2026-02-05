using EventMaster.Application.EntityRequests.Baskets.Commands.DeleteSavedEvent;
using EventMaster.Application.EntityRequests.Baskets.Commands.SaveEvent;
using EventMaster.Application.EntityRequests.Baskets.Queries.Get;
using EventMaster.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = UserRoles.Participant)]
public class BasketsController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var query = new GetSavedEventsQuery();

        var result = await Sender.Send(query);

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> Add(SaveEventCommand command)
    {
        var result = await Sender.Send(command);

        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }

    [HttpDelete("{eventId}")]
    public async Task<IActionResult> Delete(Guid eventId)
    {
        var result = await Sender.Send(new RemoveSavedEventCommand(eventId));

        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
