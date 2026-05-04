using API.Controllers.Base;
using Application.Commands.Users;
using Application.Commands.Users.Tourists;
using Application.Common.Interfaces.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
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
                result.Value.AccessTokenExpiresAtInHours,
                result.Value.RefreshTokenExpiresAtInDays);

            return Ok();
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
                result.Value.AccessTokenExpiresAtInHours,
                result.Value.RefreshTokenExpiresAtInDays);

            return Ok();
        }
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
            Secure = true, // ⚠️ MUST be true in production (HTTPS)
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(accessTokenExpiresInHours)
        };

        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // ⚠️ MUST be true in production (HTTPS)
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(refreshTokenExpiresInDays),
            Path = "/api/auth/refresh" // 👈 Scope it (better security)
        };

        response.Cookies.Append("access_token", accessToken, accessTokenOptions);
        response.Cookies.Append("refresh_token", refreshToken, refreshTokenOptions);
    }
}
