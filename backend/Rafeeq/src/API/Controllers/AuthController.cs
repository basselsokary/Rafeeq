using API.Controllers.Base;
using Application.Commands.Users;
using Application.Common.Interfaces.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.API.Controllers;

[Route("api/[controller]")]
public class AuthController() : ApiBaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        [FromServices] ICommandHandler<LoginCommand, LoginResponse> commandHandler)
    {
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        [FromServices] ICommandHandler<RegisterCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshCommand command,
        [FromServices] ICommandHandler<RefreshCommand, RefreshResponse> commandHandler)
    {
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }
}
