using API.Configurations;
using API.Controllers.Base;
using Application.Commands.Artifacts;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Artifacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting(RateLimiterPolicies.ScanImagePolicy)]
public class ScannerController : ApiBaseController
{
    [HttpPost("scan-image")]
    public async Task<ActionResult<ArtifactDetailsDto>> ScanImage(
        IFormFile file,
        [FromServices] ICommandHandler<ScanImageCommand, ArtifactDetailsDto> commandHandler,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");
        
        await using var stream = file.OpenReadStream();

        var command = new ScanImageCommand(stream, file.ContentType);
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
}
