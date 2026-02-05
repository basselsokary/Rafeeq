using EventMaster.Application.EntityRequests.Events.Commands.Create;
using EventMaster.Application.EntityRequests.Events.Commands.Delete;
using EventMaster.Application.EntityRequests.Events.Commands.Update.UpdateByAdmin;
using EventMaster.Application.EntityRequests.Events.Commands.Update.UpdateByOrganizer;
using EventMaster.Application.EntityRequests.Events.Queries.Get.GetForAdmins;
using EventMaster.Application.EntityRequests.Events.Queries.Get.GetForOrganizer;
using EventMaster.Application.EntityRequests.Events.Queries.Get.GetForParticipant;
using EventMaster.Application.EntityRequests.Events.Queries.GetById;
using EventMaster.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.API.Controllers;

[Route("api/[controller]")]
public class EventsController(ISender sender) : ApiController(sender)
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetEventByIdQuery(id);

        var result = await Sender.Send(query);

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet]
    public async Task<IActionResult> GetForParticipant([FromQuery] string? location, [FromQuery] DateTime? date)
    {
        var result = await Sender.Send(new GetEventsForParticipantQuery(location, date));

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("organizers")]
    [Authorize(Roles = UserRoles.EventOrganizer)]
    public async Task<IActionResult> GetForOrganizer()
    {
        var result = await Sender.Send(new GetEventsForOrganizerQuery());
        
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("admins")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetForAdmins()
    {
        var result = await Sender.Send(new GetEventsForAdminQuery());

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.EventOrganizer)]
    public async Task<IActionResult> Create(CreateEventCommand command)
    {
        var result = await Sender.Send(command);
        
        return result.Succeeded? Ok() : BadRequest(result.Errors);
    }

    [HttpPut("admins/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Update(Guid id, UpdateEventByAdminCommand command)
    {
        if (id != command.Id) return BadRequest("Id conflict.");

        var result = await Sender.Send(command);

        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }

    [HttpPut("organizers/{id}")]
    [Authorize(Roles = UserRoles.EventOrganizer)]
    public async Task<IActionResult> Update(Guid id, UpdateEventByOrganizerCommand command)
    {
        if (id != command.Id) return BadRequest("Id conflict.");
        
        var result = await Sender.Send(command);

        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.EventOrganizer)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Sender.Send(new DeleteEventCommand(id));

        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
