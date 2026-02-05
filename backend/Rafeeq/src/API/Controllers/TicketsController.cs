using EventMaster.Application.EntityRequests.Tickets.Commands.Purchase;
using EventMaster.Application.EntityRequests.Tickets.Queries.GetById;
using EventMaster.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace EventMaster.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class TicketsController(ISender sender) : ApiController(sender)
{
    [HttpGet("{id}")]
    [Authorize(Roles = UserRoles.Participant)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetTicketByIdQuery(id);

        var result = await Sender.Send(query);

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Participant)]
    public async Task<ActionResult<Result>> Purchase(PurchaseTicketCommand command)
    {
        var result = await Sender.Send(command);

        return result.Succeeded ? Ok() : BadRequest(result.Errors);
        // return result.Succeeded ? Created($"/Tickets/{id}", id) : BadRequest(result.Errors);
    }
}
