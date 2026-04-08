using API.Controllers.Base;
using API.RequestDTOs;
using Application.Commands.Sponsors;
using Application.Commands.Sponsors.Images;
using Application.Commands.Sponsors.Offers;
using Application.Common.Interfaces.Messaging;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admin/sponsors/")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminSponsorsController : ApiBaseController
{
	public record CreateSponsorRequest(
		string Title,
		string Description,
		SponsorType Type,
		SponsorTier Tier,
		LocationRequest Location,
		AddressRequest Address,
		DateTime StartDate,
		DateTime EndDate);

	[HttpPost]
	public async Task<IActionResult> Create(
		[FromBody] CreateSponsorRequest request,
		[FromServices] ICommandHandler<CreateSponsorCommand> commandHandler)
	{
		var command = new CreateSponsorCommand(
			request.Title,
			request.Description,
			request.Type,
			request.Tier,
			request.Location.Latitude,
			request.Location.Longitude,
			request.Address.Street,
			request.Address.City,
			request.Address.Region,
			request.Address.PostalCode,
			request.StartDate,
			request.EndDate);

		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}
	
	public record UpdateSponsorRequest(
		string Title,
		string Description,
		SponsorType Type,
		SponsorTier Tier,
		LocationRequest Location,
		AddressRequest Address,
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
			request.Location.Latitude,
			request.Location.Longitude,
			request.Address.Street,
			request.Address.City,
			request.Address.Region,
			request.Address.PostalCode,
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

	public record ActivateSponsorRequest(bool Activate);

	[HttpPatch("{id:guid}/activation")]
	public async Task<IActionResult> SetActivation(
		[FromRoute] Guid id,
		[FromBody] ActivateSponsorRequest request,
		[FromServices] ICommandHandler<ActivateSponsorCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new ActivateSponsorCommand(id, request.Activate));

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
		var command = new SetSponsorContactInfoCommand(id, request.Phone, request.Email, request.WebsiteUrl);
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

	[HttpPost("{id:guid}/images")]
	public async Task<IActionResult> AddImages(
		[FromRoute] Guid id,
		[FromBody] List<AddSponsorImageDto> request,
		[FromServices] ICommandHandler<AddSponsorImagesCommand> commandHandler)
	{
		var command = new AddSponsorImagesCommand(id, request);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}/images")]
	public async Task<IActionResult> RemoveImages(
		[FromRoute] Guid id,
		[FromRoute] List<Guid> imageIds,
		[FromServices] ICommandHandler<RemoveSponsorImagesCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new RemoveSponsorImagesCommand(id, imageIds));

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
		var command = new AddSponsorOfferCommand(
			id,
			request.Title,
			request.Description,
			request.DiscountAmount,
			request.DiscountPercentage,
			request.StartDate,
			request.EndDate,
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
		var command = new UpdateSponsorOfferCommand(
			id,
			offerId,
			request.Title,
			request.Description,
			request.DiscountAmount,
			request.DiscountPercentage,
			request.StartDate,
			request.EndDate,
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
}
