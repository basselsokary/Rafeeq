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

// namespace API.Controllers;

// [Route("api/[controller]")]
// [Authorize(Roles = UserRoles.Tourist)]
// public class ReviewsController : ApiBaseController
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

// 	[HttpGet("site/{siteId:guid}")]
// 	[AllowAnonymous]
// 	public async Task<ActionResult<PagedResult<ReviewListDto>>> GetBySiteId(
// 		[FromRoute] Guid siteId,
// 		[FromQuery] ReviewStatus? status,
// 		[FromQuery] ReviewOrderBy? sortBy,
// 		[FromQuery] int? rating,
// 		[FromServices] IQueryHandler<GetReviewsBySiteIdQuery, PagedResult<ReviewListDto>> queryHandler,
// 		[FromQuery] int page = 1,
// 		[FromQuery] int pageSize = 20,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var paging = new PagingParameters(page, pageSize);

// 		var query = new GetReviewsBySiteIdQuery(
//             siteId,
//             paging,
// 			rating,
//             status ?? ReviewStatus.Approved,
//             sortBy ?? ReviewOrderBy.Helpful);

// 		var result = await queryHandler.HandleAsync(query, cancellationToken);

// 		return HandleResult(result);
// 	}

// 	[HttpGet("me")]
// 	public async Task<ActionResult<PagedResult<TouristReviewDto>>> GetMyReviews(
// 		[FromServices] IQueryHandler<GetReviewsByUserIdQuery, PagedResult<TouristReviewDto>> queryHandler,
// 		[FromQuery] int page = 1,
// 		[FromQuery] int pageSize = 20,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var paging = new PagingParameters(page, pageSize);

// 		var result = await queryHandler.HandleAsync(new GetReviewsByUserIdQuery(paging), cancellationToken);

// 		return HandleResult(result);
// 	}

// 	[HttpPost]
// 	public async Task<IActionResult> Create(
// 		[FromBody] CreateReviewCommand command,
// 		[FromServices] ICommandHandler<CreateReviewCommand> commandHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var result = await commandHandler.HandleAsync(command, cancellationToken);

// 		return HandleResult(result);
// 	}

// 	public record UpdateReviewRequest(
// 		int Rating,
// 		string Title,
// 		string Content);

// 	[HttpPut("{id:guid}")]
// 	public async Task<IActionResult> Update(
// 		[FromRoute] Guid id,
// 		[FromBody] UpdateReviewRequest request,
// 		[FromServices] ICommandHandler<UpdateReviewCommand> commandHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var command = new UpdateReviewCommand(id, request.Rating, request.Title, request.Content);
// 		var result = await commandHandler.HandleAsync(command, cancellationToken);

// 		return HandleResult(result);
// 	}

// 	public record MarkHelpfulRequest(bool IsHelpful);

// 	[HttpPut("{id:guid}/helpful")]
// 	public async Task<IActionResult> MarkHelpful(
// 		[FromRoute] Guid id,
// 		[FromBody] MarkHelpfulRequest request,
// 		[FromServices] ICommandHandler<HelpfulReviewCommand> commandHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var result = await commandHandler.HandleAsync(new HelpfulReviewCommand(id, request.IsHelpful), cancellationToken);

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
