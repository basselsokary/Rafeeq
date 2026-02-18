using API.Controllers.Base;
using API.Services.Dispatchers;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.API.Controllers;

[Route("api/[controller]")]
public class AuthController(IRequestDispatcher dispatcher) : ApiBaseController(dispatcher)
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Dispatcher.DispatchAsync(command);

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Dispatcher.DispatchAsync(command);

        return result.Succeeded ? Ok("User registerd successfully!") : BadRequest(result.Errors);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshCommand command)
    {
        var result = await Dispatcher.DispatchAsync(command);

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }
}
