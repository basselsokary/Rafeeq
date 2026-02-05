using EventMaster.Application.EntityRequests.EventAttachments.Commands.Add;
using EventMaster.Application.EntityRequests.EventAttachments.Commands.Delete;
using EventMaster.Application.EntityRequests.EventAttachments.Queries.Get;
using EventMaster.Application.EntityRequests.EventAttachments.Queries.GetById;
using EventMaster.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.API.Controllers;

[Route("api/events")]
[Authorize]
public class EventAttachmentsController(ISender sender) : ApiController(sender)
{
    [HttpGet("{eventId}/attachments")]
    public async Task<IActionResult> GetByEventId(Guid eventId)
    {
        var query = new GetEventAttachmentsByEventIdQuery(eventId);

        var result = await Sender.Send(query);

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("{eventId}/attachments/{eventAttachmentId}")]
    //[Authorize(Roles = $"{UserRoles.EventOrganizer},{UserRoles.Admin}")]
    public async Task<IActionResult> GetById(Guid attachmentId, Guid eventId)
    {
        var query = new GetEventAttachmentByIdQuery(attachmentId, eventId);

        var result = await Sender.Send(query);

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost("{eventId}/attachments")]
    [Authorize(Roles = UserRoles.EventOrganizer)]
    public async Task<IActionResult> Create(Guid eventId, AddEventAttachmentCommand command)
    {
        if (eventId != command.EventId) return BadRequest("Id conflict");

        var result = await Sender.Send(command);

        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }

    [HttpDelete("{eventId}/attachments/{id}")]
    [Authorize(Roles = UserRoles.EventOrganizer)]
    public async Task<IActionResult> Delete(Guid attachmentId, Guid eventId)
    {
        var result = await Sender.Send(new DeleteEventAttachmentCommand(attachmentId, eventId));

        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
