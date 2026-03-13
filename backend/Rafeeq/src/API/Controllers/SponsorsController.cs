using API.Controllers.Base;
using Application.Commands.Sponsors;
using Application.Commands.Sponsors.Images;
using Application.Commands.Sponsors.Offers;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;
using Application.Queries.Sponsors;
using Application.Queries.Sponsors.Offers;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace API.Controllers;

[Route("api/[controller]")]
public class SponsorsController : ApiBaseController
{
	[HttpGet]
	public async Task<IActionResult> GetAll(
		[FromQuery] string? searchTerm,
		[FromQuery] SponsorType? type,
		[FromQuery] SponsorTier? tier,
		[FromQuery] bool? activeOnly,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetSponsorsQuery, PagedResult<SponsorListDto>> queryHandler)
	{
		var filters = new SponsorFilters(type, tier, activeOnly);
		var paging = new PagingParameters(page, pageSize);

		var query = new GetSponsorsQuery(filters, paging, searchTerm);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetSponsorByIdQuery, SponsorDetailDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetSponsorByIdQuery(id));

		return HandleResult(result);
	}

	[HttpGet("nearby")]
	public async Task<IActionResult> GetNearby(
		[FromQuery] double latitude,
		[FromQuery] double longitude,
		[FromQuery] int radiusKm,
		[FromQuery] SponsorType? type,
		[FromQuery] SponsorTier? tier,
		[FromQuery] bool? activeOnly,
		[FromServices] IQueryHandler<GetNearbySponsorsQuery, List<NearbySponsorDto>> queryHandler)
	{
		var filters = new SponsorFilters(type, tier, activeOnly);
		var query = new GetNearbySponsorsQuery(latitude, longitude, filters, RadiusKm: radiusKm);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

    [HttpGet("{id:guid}/offers")]
	public async Task<IActionResult> GetOffersBySponsor(
		[FromRoute] Guid id,
		[FromQuery] bool activeOnly,
		[FromServices] IQueryHandler<GetSiteOffersQuery, List<SponsorOfferDto>> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetSiteOffersQuery(id, activeOnly));

		return HandleResult(result);
	}

	[HttpGet("offers")]
	public async Task<IActionResult> GetAllOffers(
		[FromQuery] SponsorType? type,
		[FromQuery] SponsorTier? tier,
		[FromQuery] bool? sponsorActiveOnly,
		[FromQuery] bool activeOnly,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetAllSiteOffersAsync, PagedResult<SponsorOfferListDto>> queryHandler)
	{
		var filters = new SponsorFilters(type, tier, sponsorActiveOnly);
		var paging = new PagingParameters(page, pageSize);

		var query = new GetAllSiteOffersAsync(filters, paging, activeOnly);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

	[HttpGet("offers/{offerId:guid}")]
	public async Task<IActionResult> GetOfferById(
		[FromRoute] Guid offerId,
		[FromServices] IQueryHandler<GetSiteOfferByIdAsync, SponsorOfferDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetSiteOfferByIdAsync(offerId));

		return HandleResult(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create(
		[FromBody] CreateSponsorCommand command,
		[FromServices] ICommandHandler<CreateSponsorCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	public record UpdateSponsorRequest(
		string Title,
		string Description,
		SponsorType Type,
		SponsorTier Tier,
		GeoLocation Location,
		Address Address,
		DateTime StartDate,
		DateTime EndDate);

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(
		[FromRoute] Guid id,
		[FromBody] UpdateSponsorRequest request,
		[FromServices] ICommandHandler<UpdateSponsorCommand> commandHandler)
	{
		var command = new UpdateSponsorCommand(
			id,
			request.Title,
			request.Description,
			request.Type,
			request.Tier,
			request.Location,
			request.Address,
			request.StartDate,
			request.EndDate);

		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		[FromServices] ICommandHandler<DeleteSponsorCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new DeleteSponsorCommand(id));

		return HandleResult(result);
	}

	public record SetSponsorActivationRequest(bool Active);

	[HttpPatch("{id:guid}/activation")]
	public async Task<IActionResult> SetActivation(
		[FromRoute] Guid id,
		[FromBody] SetSponsorActivationRequest request,
		[FromServices] ICommandHandler<ActivateSponsorCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new ActivateSponsorCommand(id, request.Active));

		return HandleResult(result);
	}

	public record SetSponsorContactInfoRequest(
		string Phone,
		string Email,
		string? WebsiteUrl);

	[HttpPatch("{id:guid}/contact-info")]
	public async Task<IActionResult> SetContactInfo(
		[FromRoute] Guid id,
		[FromBody] SetSponsorContactInfoRequest request,
		[FromServices] ICommandHandler<SetSponsorContactInfoCommand> commandHandler)
	{
		var phoneResult = PhoneNumber.Create(request.Phone);
		if (phoneResult.Failed)
			return BadRequest(phoneResult.Error.Message);

		var emailResult = Email.Create(request.Email);
		if (emailResult.Failed)
			return BadRequest(emailResult.Error.Message);

		var command = new SetSponsorContactInfoCommand(id, phoneResult.Value, emailResult.Value, request.WebsiteUrl);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	public record ExtendSponsorContractRequest(DateTime NewEndDate);

	[HttpPatch("{id:guid}/contract")]
	public async Task<IActionResult> ExtendContract(
		[FromRoute] Guid id,
		[FromBody] ExtendSponsorContractRequest request,
		[FromServices] ICommandHandler<ExtendSponsorContractCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new ExtendSponsorContractCommand(id, request.NewEndDate));

		return HandleResult(result);
	}

	public record AddSponsorImageRequest(
		string ImageUrl,
		bool IsMain,
		string? Caption);

	[HttpPost("{id:guid}/images")]
	public async Task<IActionResult> AddImage(
		[FromRoute] Guid id,
		[FromBody] AddSponsorImageRequest request,
		[FromServices] ICommandHandler<AddSponsorImagesCommand> commandHandler)
	{
		var command = new AddSponsorImagesCommand(id, request.ImageUrl, request.IsMain, request.Caption);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}/images/{imageId:guid}")]
	public async Task<IActionResult> RemoveImage(
		[FromRoute] Guid id,
		[FromRoute] Guid imageId,
		[FromServices] ICommandHandler<RemoveSponsorImagesCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new RemoveSponsorImagesCommand(id, imageId));

		return HandleResult(result);
	}

	public record UpsertSponsorOfferRequest(
		string Title,
		string Description,
		decimal? DiscountAmount,
		int? DiscountPercentage,
		DateTime StartDate,
		DateTime EndDate,
		string? TermsAndConditions,
		int? MaxRedemptions);

	[HttpPost("{id:guid}/offers")]
	public async Task<IActionResult> AddOffer(
		[FromRoute] Guid id,
		[FromBody] UpsertSponsorOfferRequest request,
		[FromServices] ICommandHandler<AddSponsorOfferCommand> commandHandler)
	{
		Result<Money>? moneyResult = null;
		if (request.DiscountAmount.HasValue)
		{
			moneyResult = Money.Create(request.DiscountAmount.Value);
			if (moneyResult.Failed)
				return BadRequest(moneyResult.Error.Message);
		}

		var dateRangeResult = DateRange.Create(request.StartDate, request.EndDate);
		if (dateRangeResult.Failed)
			return BadRequest(dateRangeResult.Error.Message);

		var command = new AddSponsorOfferCommand(
			id,
			request.Title,
			request.Description,
			moneyResult?.Value,
			request.DiscountPercentage,
			dateRangeResult.Value,
			request.TermsAndConditions,
			request.MaxRedemptions);

		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	[HttpPut("{id:guid}/offers/{offerId:guid}")]
	public async Task<IActionResult> UpdateOffer(
		[FromRoute] Guid id,
		[FromRoute] Guid offerId,
		[FromBody] UpsertSponsorOfferRequest request,
		[FromServices] ICommandHandler<UpdateSponsorOfferCommand> commandHandler)
	{
		Result<Money>? moneyResult = null;
		if (request.DiscountAmount.HasValue)
		{
			moneyResult = Money.Create(request.DiscountAmount.Value);
			if (moneyResult.Failed)
				return BadRequest(moneyResult.Error.Message);
		}

		var dateRangeResult = DateRange.Create(request.StartDate, request.EndDate);
		if (dateRangeResult.Failed)
			return BadRequest(dateRangeResult.Error.Message);

		var command = new UpdateSponsorOfferCommand(
			id,
			offerId,
			request.Title,
			request.Description,
			moneyResult?.Value,
			request.DiscountPercentage,
			dateRangeResult.Value,
			request.TermsAndConditions,
			request.MaxRedemptions);

		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}/offers/{offerId:guid}")]
	public async Task<IActionResult> RemoveOffer(
		[FromRoute] Guid id,
		[FromRoute] Guid offerId,
		[FromServices] ICommandHandler<RemoveSponsorOfferCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new RemoveSponsorOfferCommand(id, offerId));

		return HandleResult(result);
	}

	public record SetOfferPromoCodeRequest(string PromoCode);

	[HttpPatch("{id:guid}/offers/{offerId:guid}/promo-code")]
	public async Task<IActionResult> SetOfferPromoCode(
		[FromRoute] Guid id,
		[FromRoute] Guid offerId,
		[FromBody] SetOfferPromoCodeRequest request,
		[FromServices] ICommandHandler<SetSponsorOfferPromoCodeCommand> commandHandler)
	{
		var command = new SetSponsorOfferPromoCodeCommand(id, offerId, request.PromoCode);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	[HttpPatch("{id:guid}/offers/{offerId:guid}/redeem")]
	public async Task<IActionResult> RedeemOffer(
		[FromRoute] Guid id,
		[FromRoute] Guid offerId,
		[FromServices] ICommandHandler<RedeemSponsorOfferCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new RedeemSponsorOfferCommand(id, offerId));

		return HandleResult(result);
	}
}
