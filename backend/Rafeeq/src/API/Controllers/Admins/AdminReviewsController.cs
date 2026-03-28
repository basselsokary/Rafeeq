using API.Controllers.Base;
using Application.Commands.Reviews;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Application.Queries.Reviews;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admin/reviews/")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminReviewsController : ApiBaseController
{
    [HttpGet("status/{status}")]
	public async Task<ActionResult<PagedResult<ReviewListDto>>> GetByStatus(
		[FromRoute] ReviewStatus status,
		[FromServices] IQueryHandler<GetReviewsByStatusQuery, PagedResult<ReviewListDto>> queryHandler,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20)
	{
		var paging = new PagingParameters(page, pageSize);

		var query = new GetReviewsByStatusQuery(paging, status);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}
    
    public record SetReviewStatusRequest(
		ReviewStatus Status,
		string? RejectionReason = null);

	[HttpPatch("{id:guid}/status")]
	public async Task<IActionResult> SetStatus(
		[FromRoute] Guid id,
		[FromBody] SetReviewStatusRequest request,
		[FromServices] ICommandHandler<SetReviewStatusCommand> commandHandler)
	{
		var command = new SetReviewStatusCommand(id, request.Status, request.RejectionReason ?? string.Empty);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

}
