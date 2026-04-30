using API.Controllers.Base;
using Application.Common.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ScannerController : ApiBaseController
{
    [HttpPost("scan-image")]
    public async Task<ActionResult<ImageResult>> ScanImage(
        IFormFile file,
        [FromServices] ScannerService scannerService,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");
        
        await using var stream = file.OpenReadStream();

        var result = await scannerService.ScanArtifactAsync(stream, file.ContentType, cancellationToken);
        return HandleResult(result);
    }
}
