using API.Configurations;
using API.Controllers.Base;
using Application.Commands.Users;
using Application.Commands.Users.Tourists;
using Application.Common.Interfaces.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

[Route("api/[controller]")]
[EnableRateLimiting(RateLimiterPolicies.AuthPerIp)]
public class AuthController : ApiBaseController
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginCommand command,
        [FromServices] ICommandHandler<LoginCommand, LoginResponse> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        if (result.Failed)
        {
            return HandleResult(result);
        }

        if (IsMobileClient(Request))
        {
            return HandleResult(result);
        }
        else
        {
            SetCookies(
                Response,
                result.Value.AccessToken,
                result.Value.RefreshToken,
                result.Value.AccessTokenExpirationInMinutes,
                result.Value.RefreshTokenExpirationInHours);

            return Ok(new {result.Value.AccessTokenExpirationInMinutes, result.Value.RefreshTokenExpirationInHours});
        }
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

    [HttpPost("web/refresh")]
    public async Task<IActionResult> RefreshWeb(
        [FromServices] ICommandHandler<RefreshCommand, RefreshResponse> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var accessToken = Request.Cookies["access_token"];
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { Message = "Tokens are missing in cookies." });
        }

        var command = new RefreshCommand(accessToken, refreshToken);
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        if (result.Failed)
        {   
            return BadRequest(result);
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

    private static bool IsMobileClient(HttpRequest request)
    {
        if (request.Headers.ContainsKey("Authorization"))
            return true;
        
        var userAgent = request.Headers.UserAgent.ToString();

        // Primary detection (Flutter / Dart)
        if (userAgent.Contains("Dart", StringComparison.OrdinalIgnoreCase))
            return true;

        // Secondary fallback (if needed later)
        if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static void SetCookies(
        HttpResponse response,
        string accessToken,
        string refreshToken,
        int accessTokenExpiresInHours,
        int refreshTokenExpiresInDays)
    {
        var accessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None, // Allow cross-site for mobile clients
            Expires = DateTime.UtcNow.AddHours(accessTokenExpiresInHours),
            Path = "/"
        };

        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None, // Allow cross-site for mobile clients
            Expires = DateTime.UtcNow.AddDays(refreshTokenExpiresInDays),
            Path = "/api/auth/web/refresh" // Scope it (better security)
        };

        response.Cookies.Delete("access_token");
        response.Cookies.Delete("refresh_token");
        
        response.Cookies.Append("access_token", accessToken, accessTokenOptions);
        response.Cookies.Append("refresh_token", refreshToken, refreshTokenOptions);
    }
}
