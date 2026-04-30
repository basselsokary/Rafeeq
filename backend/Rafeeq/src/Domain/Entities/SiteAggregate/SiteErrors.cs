using Shared;
using static Domain.Common.Constants.DomainConstants.Site;

namespace Domain.Entities.SiteAggregate;

public static class SiteErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("SITE_NOT_FOUND", $"Site with ID {id} does not exist.");
    
    public static Error IdRequired =>
        Error.Validation("SITE_ID_REQUIRED", "Site ID is required.");
    
    public static Error LocalizedIdRequired =>
        Error.Validation("SITE_LOCALIZED_ID_REQUIRED", "Site localized ID is required.");
    
    public static Error CityIdRequired =>
        Error.Validation("SITE_CITY_ID_REQUIRED", "City ID is required.");

    public static Error NameRequired =>
        Error.Validation("SITE_NAME_REQUIRED", "Site name cannot be empty.");
    
    public static Error NearestTransportationNameRequired =>
        Error.Validation("NEAREST_TRANSPORTATION_NAME_REQUIRED", "Nearest transportation name cannot be empty.");
    public static Error InvalidFacilityType =>
        Error.Validation("SITE_INVALID_FACILITY_TYPE", "One or more provided facility types are invalid.");
    public static Error FacilityAlreadyExists =>
        Error.Validation("SITE_FACILITY_ALREADY_EXISTS", "A facility with the same name already exists for this site.");
    
    public static Error ExceededNameLength =>
        Error.Validation("SITE_NAME_EXCEEDED_LENGTH", $"Site name cannot exceed {MaxNameLength} characters.");

    public static Error DescriptionRequired => 
        Error.Validation("SITE_DESCRIPTION_REQUIRED", "Site description cannot be empty.");
    
    public static Error FacilityDescriptionRequired => 
        Error.Validation("FACILITY_DESCRIPTION_REQUIRED", "Facility description cannot be empty.");
    
    public static Error ExceededDescriptionLength =>
        Error.Validation("SITE_DESCRIPTION_EXCEEDED_LENGTH", $"Site description cannot exceed {MaxDescriptionLength} characters.");
    
    public static Error PhoneRequired => 
        Error.Validation("SITE_CONTACT_PHONE_REQUIRED", "Site contact phone cannot be empty.");
    
    public static Error ImageIdRequired => 
        Error.Validation("SITE_IMAGE_ID_REQUIRED", "Site image ID is required.");
    
    public static Error ExceededImageLength => 
        Error.Validation("SITE_IMAGE_EXCEEDED_LENGTH", "Site image URL cannot exceed the maximum length.");
    
    public static Error ImageUrlRequired => 
        Error.Validation("SITE_IMAGE_URL_REQUIRED", "Site image URL is required.");
    
    public static Error ImageNotFound =>
        Error.NotFound("SITE_IMAGE_NOT_FOUND", "Site image does not exist.");

    public static Error CaptionRequired =>
        Error.Validation("SITE_CAPTION_REQUIRED", "Site image caption is required.");
    
    public static Error NegativeDisplayOrder => 
        Error.Validation("SITE_NEGATIVE_DISPLAY_ORDER", "Site display order cannot be negative.");

    public static Error LocalizedContentNotFound =>
        Error.NotFound("SITE_LOCALIZED_CONTENT_NOT_FOUND", "Site localized content does not exist.");
    
    public static Error FacilityLocalizedContentNotFound =>
        Error.NotFound("FACILITY_LOCALIZED_CONTENT_NOT_FOUND", "Facility localized content does not exist.");
    
    public static Error NearestTransportationLocalizedContentNotFound =>
        Error.NotFound("NEAREST_TRANSPORTATION_LOCALIZED_CONTENT_NOT_FOUND", "Nearest transportation localized content does not exist.");

    public static Error FacilityIdRequired => 
        Error.Validation("SITE_FACILITY_ID_REQUIRED", "Site facility ID is required.");
    
    public static Error FacilityNotFound =>
        Error.NotFound("SITE_FACILITY_NOT_FOUND", "Site facility does not exist.");
    public static Error TransportationNotFound =>
        Error.NotFound("SITE_NEAREST_TRANSPORTATION_NOT_FOUND", "Site nearest transportation does not exist.");

    public static Error OpeningHourNotFound =>
        Error.NotFound("SITE_OPENING_HOUR_NOT_FOUND", "Site opening hour does not exist.");
        
    public static Error TransportationWithSameLocationAlreadyExists =>
        Error.Validation("SITE_NEAREST_TRANSPORTATION_DUPLICATE_LOCATION", "A nearest transportation with the same location already exists for this site.");

    public static Error CannotBeFeaturedAndHiddenGem =>
        Error.Validation("SITE_CANNOT_BE_FEATURED_AND_HIDDEN_GEM", "A site cannot be both a featured attraction and a hidden gem.");

    public static Error InvalidEstimatedDuration =>
        Error.Validation("SITE_INVALID_ESTIMATED_DURATION", "Estimated duration must be greater than zero.");
}