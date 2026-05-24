using API.Controllers.Base;
using API.DTOs;
using Application.Commands.Sponsors;
using Application.Commands.Sponsors.Images;
using Application.Commands.Sponsors.LocalizedContents;
using Application.Commands.Sponsors.Offers;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Admins;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;
using Application.Queries.Sponsors;
using Application.Queries.Sponsors.Images;
using Application.Queries.Sponsors.LocalizedContents;
using Application.Queries.Sponsors.Offers;
using Application.Services;
using Domain.Enums;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admins/sponsors/")]
[Authorize(Policy = Policies.CanManageSponsors)]
public class AdminSponsorsController : ApiBaseController
{
	#region Basic CRUD Operations
	[HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminSponsorDetailDto>> GetById(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSponsorByIdForAdminQuery, AdminSponsorDetailDto> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSponsorByIdForAdminQuery(id);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

	public record CreateSponsorRequest(string Title, string Description, string Address, SponsorType Type, SponsorTier Tier, LocationRequest Location, DateTime StartDate, DateTime EndDate, string? WebsiteUrl, string? ContactPhone, string? ContactEmail);

	[HttpPost]
	public async Task<IActionResult> Create(
		[FromBody] CreateSponsorRequest request,
		[FromServices] ICommandHandler<CreateSponsorCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var command = new CreateSponsorCommand(
			request.Title,
			request.Description,
			request.Address,
			request.Type,
			request.Tier,
			request.Location.Latitude,
			request.Location.Longitude,
			request.StartDate,
			request.EndDate,
			request.WebsiteUrl,
			request.ContactPhone,
			request.ContactEmail);

		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	
	public record UpdateSponsorRequest(SponsorType Type, SponsorTier Tier, LocationRequest Location, DateTime? NewEndDate, string? ContactPhone, string? ContactEmail, string? WebsiteUrl);

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(
		[FromRoute] Guid id,
		[FromBody] UpdateSponsorRequest request,
		[FromServices] ICommandHandler<UpdateSponsorCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var command = new UpdateSponsorCommand(
			id,
			request.Type,
			request.Tier,
			request.Location.Latitude,
			request.Location.Longitude,
			request.NewEndDate,
			request.ContactPhone,
			request.ContactEmail,
			request.WebsiteUrl);

		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		[FromServices] ICommandHandler<DeleteSponsorCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var result = await commandHandler.HandleAsync(new DeleteSponsorCommand(id), cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Partial Updates (PATCH)
	public record ActivateSponsorRequest(bool Activate);

	[HttpPatch("{id:guid}")]
	public async Task<IActionResult> SetActivation(
		[FromRoute] Guid id,
		[FromBody] ActivateSponsorRequest request,
		[FromServices] ICommandHandler<ActivateSponsorCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var command = new ActivateSponsorCommand(id, request.Activate);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	[HttpPatch("{id:guid}/set-main-image/{imageId:guid}")]
	public async Task<IActionResult> SetMainImage(
		[FromRoute] Guid id,
		[FromRoute] Guid imageId,
		[FromServices] ICommandHandler<SetMainSponsorImageCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new SetMainSponsorImageCommand(id, imageId);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Images
	[HttpGet("{id:guid}/images")]
	public async Task<ActionResult<List<ImageDto>>> GetImagesBySponsorId(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetSponsorImagesByIdQuery, List<ImageDto>> queryHandler,
        CancellationToken cancellationToken = default)
	{
		var query = new GetSponsorImagesByIdQuery(id);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	[HttpGet("{id:guid}/images/{imageId:guid}")]
	public async Task<ActionResult<ImageDto>> GetImageById(
		[FromRoute] Guid id,
		[FromRoute] Guid imageId,
		[FromServices] IQueryHandler<GetSponsorImageByIdQuery, ImageDto> queryHandler,
        CancellationToken cancellationToken = default)
	{
		var query = new GetSponsorImageByIdQuery(id, imageId);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	public sealed record AddSponsorImagesRequest(List<AddSponsorImageItem> Images);
    public sealed record AddSponsorImageItem(IFormFile Image, bool IsMain, int DisplayOrder, string? Caption);

    [HttpPost("{id:guid}/images")]
    public async Task<IActionResult> AddImages(
        [FromRoute] Guid id,
        [FromForm] AddSponsorImagesRequest request,
        [FromServices] ICommandHandler<AddSponsorImagesCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
		var images = new List<AddSponsorImageDto>();

		foreach (var item in request.Images)
		{
			var imageStream = new MemoryStream();
			await item.Image.CopyToAsync(imageStream, cancellationToken);
			imageStream.Position = 0;

			images.Add(new AddSponsorImageDto(
				new FileUploadInput(imageStream, item.Image.FileName, item.Image.Length),
				item.IsMain,
				item.DisplayOrder,
				item.Caption));
		}

        var command = new AddSponsorImagesCommand(id, images);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

	[HttpDelete("{id:guid}/images")]
	public async Task<IActionResult> RemoveImages(
		[FromRoute] Guid id,
		[FromBody] List<Guid> imageIds,
		[FromServices] ICommandHandler<RemoveSponsorImagesCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var result = await commandHandler.HandleAsync(new RemoveSponsorImagesCommand(id, imageIds), cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Offers
	[HttpGet("{id:guid}/offers")]
	public async Task<ActionResult<List<SponsorOfferDto>>> GetOffersBySponsorId(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetSponsorOffersByIdQuery, List<SponsorOfferDto>> queryHandler,
        CancellationToken cancellationToken = default)
	{
		var query = new GetSponsorOffersByIdQuery(id);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	[HttpGet("offers/{offerId:guid}")]
	public async Task<ActionResult<SponsorOfferDto>> GetOfferById(
		[FromRoute] Guid offerId,
		[FromServices] IQueryHandler<GetSponsorOfferByIdQuery, SponsorOfferDto> queryHandler,
        CancellationToken cancellationToken = default)
	{
		var query = new GetSponsorOfferByIdQuery(offerId);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	public sealed record AddSponsorOfferRequest(decimal? DiscountAmount, int? DiscountPercentage, DateTime StartDate, DateTime EndDate, int? MaxRedemptions, string? PromoCode, List<AddOfferLocalizedContentDto> LocalizedContents);

	[HttpPost("{id:guid}/offers")]
	public async Task<IActionResult> AddOffer(
		[FromRoute] Guid id,
		[FromBody] AddSponsorOfferRequest request,
		[FromServices] ICommandHandler<AddSponsorOfferCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var command = new AddSponsorOfferCommand(
			id,
			request.DiscountAmount,
			request.DiscountPercentage,
			request.StartDate,
			request.EndDate,
			request.MaxRedemptions,
			request.PromoCode,
			request.LocalizedContents);

		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	public sealed record UpdateSponsorOfferRequest(decimal? DiscountAmount, int? DiscountPercentage, DateTime StartDate, DateTime EndDate, int? MaxRedemptions, string? PromoCode);

	[HttpPut("{id:guid}/offers/{offerId:guid}")]
	public async Task<IActionResult> UpdateOffer(
		[FromRoute] Guid id,
		[FromRoute] Guid offerId,
		[FromBody] UpdateSponsorOfferRequest request,
		[FromServices] ICommandHandler<UpdateSponsorOfferCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var command = new UpdateSponsorOfferCommand(
			id,
			offerId,
			request.DiscountAmount,
			request.DiscountPercentage,
			request.StartDate,
			request.EndDate,
			request.MaxRedemptions,
			request.PromoCode);

		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}/offers/{offerId:guid}")]
	public async Task<IActionResult> RemoveOffer(
		[FromRoute] Guid id,
		[FromRoute] Guid offerId,
		[FromServices] ICommandHandler<RemoveSponsorOfferCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var command = new RemoveSponsorOfferCommand(id, offerId);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Localized Contents
	[HttpGet("{id:guid}/localized-contents")]
    public async Task<ActionResult<List<AdminSponsorLocalizedContentDto>>> GetLocalizedContents(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSponsorLocalizedContentsQuery, List<AdminSponsorLocalizedContentDto>> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSponsorLocalizedContentsQuery(id);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }
	[HttpPost("{id:guid}/localized-contents")]
    public async Task<IActionResult> AddLocalizedContents(
        [FromRoute] Guid id,
        [FromBody] List<AddSponsorLocalizedContentsDtoCommand> request,
        [FromServices] ICommandHandler<AddSponsorLocalizedContentsCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new AddSponsorLocalizedContentsCommand(id, request);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

	[HttpPut("{id:guid}/localized-contents")]
	public async Task<IActionResult> UpdateLocalizedContents(
		[FromRoute] Guid id,
		[FromBody] List<UpdateSponsorLocalizedContentsDto> request,
		[FromServices] ICommandHandler<UpdateSponsorLocalizedContentCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var command = new UpdateSponsorLocalizedContentCommand(id, request);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Dashboard & Statistics
	[HttpGet("dashboard")]
    public async Task<ActionResult<AdminSponsorDashboardDto>> GetDashboard(
        [FromServices] IQueryHandler<GetSponsorsDashboardQuery, AdminSponsorDashboardDto> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSponsorsDashboardQuery();
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }
	#endregion
}
