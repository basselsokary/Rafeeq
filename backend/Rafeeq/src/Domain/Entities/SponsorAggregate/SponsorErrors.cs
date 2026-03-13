using Domain.Common.Constants;
using Shared.Models;

namespace Domain.Entities.SponsorAggregate;

public class SponsorErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("SPONSOR_NOT_FOUND", $"Sponsor with ID {id} does not exist.");
    
    public static Error IdRequired =>
        Error.Validation("SPONSOR_EMPTY_ID", "Sponsor ID cannot be empty.");
    
    public static Error OfferIdRequired =>
        Error.Validation("SPONSOR_EMPTY_OFFER_ID", "Sponsor Offer ID cannot be empty.");

    public static Error OfferNotFound(Guid offerId) =>
        Error.NotFound("SPONSOR_NOT_FOUND", $"Offer with ID {offerId} does not exist.");

    public static Error TitleRequired => 
        Error.Validation("SPONSOR_TITLE_REQUIRED", "Sponsor title cannot be empty.");

    public static Error DescriptionRequired => 
        Error.Validation("SPONSOR_DESC_REQUIRED", "Sponsor description cannot be empty.");
    
    public static Error PromoCodeRequired => 
        Error.Validation("SPONSOR_PROMO_CODE_REQUIRED", "Promo code cannot be empty.");
    
    public static Error DiscountRequired => 
        Error.Validation("SPONSOR_DISCOUNT_REQUIRED", "Either discount amount or percentage must be provided.");
    
    public static Error DiscountPercentageInvalid => 
        Error.Validation("SPONSOR_DISCOUNT_PCT_INVALID", "Discount percentage must be between 0 and 100.");

    public static Error InactiveOffer => 
        Error.Validation("SPONSOR_OFFER_INACTIVE", "Cannot redeem an inactive offer.");
    
    public static Error ExpiredOffer => 
        Error.Validation("SPONSOR_OFFER_EXPIRED", "Cannot redeem an expired offer.");
    
    public static Error ExpiredContract => 
        Error.Validation("SPONSOR_EXPIRED_CONTRACT", "Cannot activate sponsor with expired contract.");
    
    public static Error MaximumRedemptionsReached => 
        Error.Failure("SPONSOR_OFFER_MAX_REDEMPTIONS_REACHED", "Maximum redemptions reached for this offer.");
    
    public static Error NegativeRedemptionsNumber => 
        Error.Validation("SPONSOR_NEGATIVE_REDEMPTIONS", "Redemptions cannot be negative.");
    
    public static Error ImageIdRequired => 
        Error.Validation("SPONSOR_EMPTY_IMAGE_ID", "Sponsor image ID cannot be empty.");

    public static Error ImageUrlRequired => 
        Error.Validation("SPONSOR_IMG_URL_REQUIRED", "Sponsor image URL cannot be empty.");
    
    public static Error ImageNotFound =>
        Error.NotFound("SPONSOR_IMG_NOT_FOUND", "Sponsor image does not exist.");
    
    public static Error NegativeDisplayOrder => 
        Error.Validation("SPONSOR_NEGATIVE_ORDER", "Sponsor display order cannot be negative.");

    public static Error LocalizedContentNotFound =>
        Error.NotFound("SPONSOR_LOCALIZED_CONTENT_NOT_FOUND", "Sponsor localized content does not exist.");

    public static Error InvalidDate =>
        Error.Validation("SPONSOR_INVALID_DATE", "Contract start date must be before end date.");
    
    public static Error InvalidExtendDate =>
        Error.Validation("SPONSOR_INVALID_EXTEND_DATE", "New end date must be after current end date.");

    public static Error InvalidAverageRating =>
        Error.Validation(
            "SPONSOR_INVALID_AVERAGE_RATING", $"Average rating must be between 0 and {DomainConstants.Review.MaxRatingValue}.");
}
