using Domain.Common.Constants;
using Shared;
using static Domain.Common.Constants.DomainConstants.Sponsor;

namespace Domain.Entities.SponsorAggregate;

public class SponsorErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("SPONSOR_NOT_FOUND", $"Sponsor with ID {id} does not exist.");
    
    public static Error IdRequired =>
        Error.Validation("SPONSOR_ID_REQUIRED", "Sponsor ID is required.");
    
    public static Error OfferIdRequired =>
        Error.Validation("SPONSOR_OFFER_ID_REQUIRED", "Sponsor offer ID is required.");

    public static Error OfferNotFound(Guid offerId) =>
        Error.NotFound("SPONSOR_OFFER_NOT_FOUND", $"Offer with ID {offerId} does not exist.");

    public static Error TitleRequired => 
        Error.Validation("SPONSOR_TITLE_REQUIRED", "Sponsor title cannot be empty.");

    public static Error ExceededTitleLength =>
        Error.Validation("SPONSOR_EXCEEDED_TITLE_LENGTH", $"Sponsor title cannot exceed {MaxTitleLength} characters.");

    public static Error DescriptionRequired => 
        Error.Validation("SPONSOR_DESCRIPTION_REQUIRED", "Sponsor description is required.");
    
    public static Error ExceededDescriptionLength =>
        Error.Validation("SPONSOR_EXCEEDED_DESCRIPTION_LENGTH", $"Sponsor description cannot exceed {MaxDescriptionLength} characters.");
    
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
        Error.Validation("SPONSOR_OFFER_MAX_REDEMPTIONS_REACHED", "Maximum redemptions reached for this offer.");
    
    public static Error NegativeRedemptionsNumber => 
        Error.Validation("SPONSOR_NEGATIVE_REDEMPTIONS", "Redemptions cannot be negative.");
    
    public static Error ImageIdRequired => 
        Error.Validation("SPONSOR_IMAGE_ID_REQUIRED", "Sponsor image ID is required.");

    public static Error ImageUrlRequired => 
        Error.Validation("SPONSOR_IMG_URL_REQUIRED", "Sponsor image URL cannot be empty.");
    
    public static Error ImageNotFound =>
        Error.NotFound("SPONSOR_IMG_NOT_FOUND", "Sponsor image does not exist.");
    
    public static Error ExceededImageLength =>
        Error.Validation("SPONSOR_EXCEEDED_IMAGE_LENGTH", "Sponsor image length exceeds the maximum allowed.");
    
    public static Error ExceededWebsiteUrlLength =>
        Error.Validation("SPONSOR_EXCEEDED_WEBSITE_URL_LENGTH", "Sponsor website URL cannot exceed the maximum allowed.");

    public static Error CaptionRequired =>
        Error.Validation("SPONSOR_CAPTION_REQUIRED", "Sponsor image caption is required.");
    
    public static Error NegativeDisplayOrder => 
        Error.Validation("SPONSOR_NEGATIVE_DISPLAY_ORDER", "Sponsor display order cannot be negative.");

    public static Error LocalizedContentNotFound =>
        Error.NotFound("SPONSOR_LOCALIZED_CONTENT_NOT_FOUND", "Sponsor localized content does not exist.");

    public static Error InvalidDate =>
        Error.Validation("SPONSOR_INVALID_DATE", "Contract start date must be before end date.");
    
    public static Error InvalidExtendDate =>
        Error.Validation("SPONSOR_INVALID_EXTEND_DATE", "New end date must be after current end date.");

    public static Error InvalidAverageRating =>
        Error.Validation("SPONSOR_INVALID_AVERAGE_RATING", $"Average rating must be between 0 and {DomainConstants.Review.MaxRatingValue}.");

    public static Error NewEndDateMustBeLater =>
        Error.Validation("SPONSOR_INVALID_NEW_DATE", "New end date must be later than current end date.");

    public static Error ContentIdRequired =>
        Error.Validation("SPONSOR_CONTENT_ID_REQUIRED", "Content ID is required.");
}
