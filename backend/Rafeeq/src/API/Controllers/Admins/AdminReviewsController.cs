// using API.Controllers.Base;
// using Application.Commands.Reviews;
// using Application.Common.Interfaces.Messaging;
// using Application.DTOs.Common;
// using Application.DTOs.Reviews;
// using Application.Queries.Reviews;
// using Domain.Common.Constants;
// using Domain.Enums;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

// namespace API.Controllers.Admins;

// [Route("api/admins/reviews/")]
// [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Moderator}")]
// public class AdminReviewsController : ApiBaseController
// {
// 	[HttpGet("{id:guid}")]
// 	public async Task<ActionResult<ReviewDetailDto>> GetById(
// 		[FromRoute] Guid id,
// 		[FromServices] IQueryHandler<GetReviewByIdQuery, ReviewDetailDto> queryHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var result = await queryHandler.HandleAsync(new GetReviewByIdQuery(id), cancellationToken);

// 		return HandleResult(result);
// 	}
	
//     [HttpGet("status/{status}")]
// 	public async Task<ActionResult<PagedResult<ReviewListDto>>> GetByStatus(
// 		[FromRoute] ReviewStatus status,
// 		[FromServices] IQueryHandler<GetReviewsByStatusQuery, PagedResult<ReviewListDto>> queryHandler,
// 		[FromQuery] int page = 1,
// 		[FromQuery] int pageSize = 20,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var paging = new PagingParameters(page, pageSize);

// 		var query = new GetReviewsByStatusQuery(paging, status);
// 		var result = await queryHandler.HandleAsync(query, cancellationToken);

// 		return HandleResult(result);
// 	}
    
//     public record SetReviewStatusRequest(
// 		ReviewStatus Status,
// 		string? RejectionReason = null);

// 	[HttpPatch("{id:guid}/status")]
// 	public async Task<IActionResult> SetStatus(
// 		[FromRoute] Guid id,
// 		[FromBody] SetReviewStatusRequest request,
// 		[FromServices] ICommandHandler<SetReviewStatusCommand> commandHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var command = new SetReviewStatusCommand(id, request.Status, request.RejectionReason ?? string.Empty);
// 		var result = await commandHandler.HandleAsync(command, cancellationToken);

// 		return HandleResult(result);
// 	}

// 	[HttpDelete("{id:guid}")]
// 	public async Task<IActionResult> Delete(
// 		[FromRoute] Guid id,
// 		[FromServices] ICommandHandler<DeleteReviewCommand> commandHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var result = await commandHandler.HandleAsync(new DeleteReviewCommand(id), cancellationToken);

// 		return HandleResult(result);
// 	}
// }
