using API.Controllers.Base;
using Application.Commands.Artifacts;
using Application.Common.Interfaces.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admins/artifacts/")]
// [Authorize(Roles = UserRoles.Admin)]
public class AdminArtifactsController : ApiBaseController
{
	[HttpPost("import")]
    // [Authorize(Policy = "SuperAdminOnly")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportArtifactsResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImportArtifactsResultDto>> ImportArtifacts(
        IFormFile file,
        [FromServices] ICommandHandler<ImportArtifactsCommand, ImportArtifactsResultDto> commandHandler,
        [FromQuery] bool dryRun = true)
    {
        await using var stream = file.OpenReadStream(); 
        var command = new ImportArtifactsCommand(stream, file.FileName, dryRun);
        var result = await commandHandler.HandleAsync(command);
        
        return HandleResult(result);
    }
}
