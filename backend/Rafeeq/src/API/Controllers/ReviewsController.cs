using API.Controllers.Base;
using Application.Commands.Reviews;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Application.Queries.Reviews;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class ReviewsController : ApiBaseController
{
	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetReviewByIdQuery, ReviewDetailDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetReviewByIdQuery(id));

		return HandleResult(result);
	}

	[HttpGet("site/{siteId:guid}")]
	public async Task<IActionResult> GetBySiteId(
		[FromRoute] Guid siteId,
		[FromQuery] ReviewStatus? status,
		[FromQuery] ReviewOrderBy? sortBy,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetReviewsBySiteIdQuery, PagedResult<ReviewListDto>> queryHandler)
	{
		var paging = new PagingParameters(page, pageSize);

		var query = new GetReviewsBySiteIdQuery(
            siteId,
            paging,
            status ?? ReviewStatus.Approved,
            sortBy ?? ReviewOrderBy.Helpful);

		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

	[HttpGet("site/{siteId:guid}/rating/{rating:int}")]
	public async Task<IActionResult> GetBySiteIdAndRating(
		[FromRoute] Guid siteId,
		[FromRoute] int rating,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetReviewsBySiteIdAndRatingQuery, PagedResult<ReviewListDto>> queryHandler)
	{
		var paging = new PagingParameters(
			page <= 0 ? 1 : page,
			pageSize <= 0 ? 20 : pageSize);

		var query = new GetReviewsBySiteIdAndRatingQuery(siteId, rating, paging);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

	[HttpGet("status/{status}")]
	public async Task<IActionResult> GetByStatus(
		[FromRoute] ReviewStatus status,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetReviewsByStatusQuery, PagedResult<ReviewListDto>> queryHandler)
	{
		var paging = new PagingParameters(page, pageSize);

		var query = new GetReviewsByStatusQuery(paging, status);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

	[HttpGet("me")]
	public async Task<IActionResult> GetMyReviews(
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetReviewsByUserIdQuery, PagedResult<UserReviewDto>> queryHandler)
	{
		var paging = new PagingParameters(
			page <= 0 ? 1 : page,
			pageSize <= 0 ? 20 : pageSize);

		var result = await queryHandler.HandleAsync(new GetReviewsByUserIdQuery(paging));

		return HandleResult(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create(
		[FromBody] CreateReviewCommand command,
		[FromServices] ICommandHandler<CreateReviewCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	public record UpdateReviewRequest(
		Rating Rating,
		string Title,
		string Content);

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(
		[FromRoute] Guid id,
		[FromBody] UpdateReviewRequest request,
		[FromServices] ICommandHandler<UpdateReviewCommand> commandHandler)
	{
		var command = new UpdateReviewCommand(id, request.Rating, request.Title, request.Content);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	public record SetReviewStatusRequest(
		ReviewStatus Status,
		string RejectionReason = "");

	[HttpPatch("{id:guid}/status")]
	public async Task<IActionResult> SetStatus(
		[FromRoute] Guid id,
		[FromBody] SetReviewStatusRequest request,
		[FromServices] ICommandHandler<SetReviewStatusCommand> commandHandler)
	{
		var command = new SetReviewStatusCommand(id, request.Status, request.RejectionReason);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	public record MarkHelpfulRequest(bool IsHelpful);

	[HttpPatch("{id:guid}/helpful")]
	public async Task<IActionResult> MarkHelpful(
		[FromRoute] Guid id,
		[FromBody] MarkHelpfulRequest request,
		[FromServices] ICommandHandler<HelpfulReviewCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new HelpfulReviewCommand(id, request.IsHelpful));

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		[FromServices] ICommandHandler<DeleteReviewCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new DeleteReviewCommand(id));

		return HandleResult(result);
	}
}
