using EventMaster.Application.EntityRequests.Auth.Commands.OrganizerStatus;
using EventMaster.Application.EntityRequests.Auth.Commands.Refresh;
using EventMaster.Application.EntityRequests.Auth.Queries.GetUsers.GetOrganizersForAdmin;
using EventMaster.Application.EntityRequests.Users.Commands.Login;
using EventMaster.Application.EntityRequests.Users.Commands.Register;
using EventMaster.Application.EntityRequests.Users.Queries.GetUsers.GetParticipantsForAdmin;
using EventMaster.Domain.Constants;
using EventMaster.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.API.Controllers;

[Route("api/[controller]")]
public class AuthController(ISender sender) : ApiController(sender)
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Sender.Send(command);

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Sender.Send(command);

        return result.Succeeded ? Ok("User registerd successfully!") : BadRequest(result.Errors);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshCommand command)
    {
        var result = await Sender.Send(command);

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("admin/show-organizers")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetOrganizers(OrganizerStatus status)
    {
        var result = await Sender.Send(new GetOrganizersForAdminsQuery(status));

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("admin/show-participants")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetParticipants()
    {
        var result = await Sender.Send(new GetParticipantsForAdminQuery());

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPut("admin/handle-organizer-status/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> HandleOrganizerStatus(string id, ChangeOrganizerStatusCommand command)
    {
        if (id != command.OrganizerId) return Conflict("Id conflict.");

        var result = await Sender.Send(command);

        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }
}
