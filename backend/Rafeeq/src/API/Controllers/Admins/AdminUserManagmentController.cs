using API.Controllers.Base;
using Application.Commands.Users.Admins;
using Application.Common.Interfaces.Messaging;
using Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admins")]
[Authorize(Roles = UserRoles.Admin)]
public class AdminUserManagmentController : ApiBaseController
{
    [HttpDelete("users")]
    public async Task<IActionResult> RemoveUser(
        string email,
        [FromServices] ICommandHandler<DeleteUserCommand> deleteUserCommandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteUserCommand(email);
        var result = await deleteUserCommandHandler.HandleAsync(command, cancellationToken);
        
        return HandleResult(result);
    }

}
