using API.Configurations;
using API.Controllers.Base;
using Application.Commands.Auth;
using Application.Common.Interfaces.Messaging;
using Application.Queries.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

[Route("api/[controller]")]
[EnableRateLimiting(RateLimiterPolicies.AuthPerIp)]
public class AuthController : ApiBaseController
{
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentUserResponse>> GetCurrentUser(
        [FromServices] IQueryHandler<GetCurrentUserQuery, CurrentUserResponse> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await queryHandler.HandleAsync(new GetCurrentUserQuery(), cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginCommand command,
        [FromServices] ICommandHandler<LoginCommand, LoginResponse> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("admins/login")]
    public async Task<ActionResult<AdminLoginResponse>> AdminLogin(
        [FromBody] AdminLoginCommand command,
        [FromServices] ICommandHandler<AdminLoginCommand, AdminLoginResponse> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        if (result.Failed)
        {
            return HandleResult(result);
        }
        
        SetCookies(
            Response,
            result.Value.AccessToken,
            result.Value.RefreshToken,
            result.Value.AccessTokenExpirationInMinutes,
            result.Value.RefreshTokenExpirationInHours);

        return Ok(new {result.Value.AccessTokenExpirationInMinutes, result.Value.RefreshTokenExpirationInHours});
    }

    [HttpPost("login-google")]
    public async Task<ActionResult<LoginWithGoogleResponse>> GoogleLogin(
        [FromBody] LoginWithGoogleCommand command,
        [FromServices] ICommandHandler<LoginWithGoogleCommand, LoginWithGoogleResponse> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        [FromServices] ICommandHandler<RegisterCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshResponse>> Refresh(
        [FromBody] RefreshCommand command,
        [FromServices] ICommandHandler<RefreshCommand, RefreshResponse> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        
        return HandleResult(result);
    }

    [HttpPost("admins/refresh")]
    public async Task<IActionResult> AdminRefresh(
        [FromServices] ICommandHandler<RefreshCommand, RefreshResponse> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { Message = "Tokens are missing in cookies." });
        }

        var command = new RefreshCommand(refreshToken);
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        if (result.Failed)
        {   
            return HandleResult(result.Error);
        }

        SetCookies(
            Response,
            result.Value.AccessToken,
            result.Value.RefreshToken,
            result.Value.AccessTokenExpirationInMinutes,
            result.Value.RefreshTokenExpirationInHours);

        return Ok(new {result.Value.AccessTokenExpirationInMinutes, result.Value.RefreshTokenExpirationInHours});
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        [FromServices] ICommandHandler<ForgotPasswordCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        [FromServices] ICommandHandler<ResetPasswordCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] ConfirmEmailCommand command,
        [FromServices] ICommandHandler<ConfirmEmailCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);
    
        return HandleResult(result);
    }

    [HttpPost("resend-email-verification")]
    public async Task<IActionResult> ResendEmailVerification(
        [FromBody] ResendEmailVerificationCommand command,
        [FromServices] ICommandHandler<ResendEmailVerificationCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        [FromServices] ICommandHandler<ChangePasswordCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return Ok();
    }

    private static void SetCookies(
        HttpResponse response,
        string accessToken,
        string refreshToken,
        int AccessTokenExpirationInMinutes,
        int RefreshTokenExpirationInHours)
    {
        var accessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(AccessTokenExpirationInMinutes),
            Path = "/"
        };

        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(RefreshTokenExpirationInHours),
            Path = "/api/auth/admins/refresh" // Better security by restricting the refresh token to a specific endpoint
        };

        response.Cookies.Delete("access_token");
        response.Cookies.Delete("refresh_token");
        
        response.Cookies.Append("access_token", accessToken, accessTokenOptions);
        response.Cookies.Append("refresh_token", refreshToken, refreshTokenOptions);
    }
}
