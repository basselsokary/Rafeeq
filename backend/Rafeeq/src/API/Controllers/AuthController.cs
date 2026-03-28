using API.Controllers.Base;
using Application.Commands.Users;
using Application.Common.Interfaces.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class AuthController() : ApiBaseController
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
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
    public async Task<ActionResult<RefreshResponse>> Refresh(
        [FromBody] RefreshCommand command,
        [FromServices] ICommandHandler<RefreshCommand, RefreshResponse> commandHandler)
    {
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        [FromServices] ICommandHandler<ForgotPasswordCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        [FromServices] ICommandHandler<ResetPasswordCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        [FromQuery] string token,
        [FromServices] ICommandHandler<ConfirmEmailCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(new ConfirmEmailCommand(token));
    
        return HandleResult(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        [FromServices] ICommandHandler<ChangePasswordCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }
}
